using System;
using OtpNet;

namespace CyreneAuth.Services
{
    public class TotpService
    {
        public static string GenerateTotp(string secret, int digits = 6, int period = 30, string algorithm = "SHA1")
        {
            try
            {
                var secretBytes = Base32Encoding.ToBytes(secret);
                var hashMode = GetHashMode(algorithm);
                var totp = new Totp(secretBytes, period, hashMode, digits);

                return totp.ComputeTotp();
            }
            catch (Exception ex)
            {
                throw new Exception("TOTP Generate Failed: ", ex);
            }
        }

        public static int GetRemainingSeconds(int period = 30)
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            int remainder = (int)(timestamp % period);
            return remainder == 0 ? period : period - remainder;
        }

        public static bool ValidateTotp(string secret, string code, int digits = 6, int period = 30, string algorithm = "SHA1", int allowOffset = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(code)) return false;

                var secretBytes = Base32Encoding.ToBytes(secret);
                var hashMode = GetHashMode(algorithm);
                var totp = new Totp(secretBytes, period, hashMode, digits);

                return totp.VerifyTotp(code, out _, new VerificationWindow(allowOffset, allowOffset));
            }
            catch (Exception ex)
            {
                throw new Exception("TOTP Validate Failed: ", ex);
            }
        }

        public static string GenerateRandomSecret(int length = 20)
        {
            var key = KeyGeneration.GenerateRandomKey(length);
            return Base32Encoding.ToString(key);
        }

        public static string GenerateTotpUri(string issuer, string accName, string secret, int digits = 6, int period = 30, string algorithm = "SHA1")
        {
            var enIssuer = Uri.EscapeDataString(issuer);
            var enAccName = Uri.EscapeDataString(accName);

            return $"otpauth://totp/{enIssuer}:{enAccName}?secret={secret}&issuer={enIssuer}&algorithm={algorithm}&digits={digits}&period={period}";
        }

        private static OtpHashMode GetHashMode(string algorithm)
        {
            return algorithm.ToUpper() switch
            {
                "SHA256" => OtpHashMode.Sha256,
                "SHA512" => OtpHashMode.Sha512,
                _ => OtpHashMode.Sha1,
            };
        }
    }
} 