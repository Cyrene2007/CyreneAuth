using System;
using OtpNet;

namespace CyreneAuth.Services
{
    public class TotpService
    {
        /// <summary>
        /// 生成TOTP验证码
        /// </summary>
        /// <param name="secret">密钥（Base32编码）</param>
        /// <param name="digits">验证码位数</param>
        /// <param name="period">验证码更新周期（秒）</param>
        /// <param name="algorithm">哈希算法（SHA1/SHA256/SHA512）</param>
        /// <returns>TOTP验证码</returns>
        public static string GenerateTotp(string secret, int digits = 6, int period = 30, string algorithm = "SHA1")
        {
            try
            {
                // 解码Base32密钥
                var secretBytes = Base32Encoding.ToBytes(secret);
                
                // 选择哈希算法
                OtpHashMode hashMode = GetHashMode(algorithm);
                
                // 创建TOTP实例
                var totp = new Totp(secretBytes, period, hashMode, digits);
                
                // 生成当前验证码
                return totp.ComputeTotp();
            }
            catch (Exception)
            {
                // 如果密钥无效或发生其他错误，返回空字符串
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取当前验证码的剩余有效时间（秒）
        /// </summary>
        /// <param name="period">验证码更新周期（秒）</param>
        /// <returns>剩余秒数</returns>
        public static int GetRemainingSeconds(int period = 30)
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            int remainder = (int)(timestamp % period);
            return remainder == 0 ? period : period - remainder;
        }

        /// <summary>
        /// 验证TOTP验证码
        /// </summary>
        /// <param name="secret">密钥（Base32编码）</param>
        /// <param name="code">用户输入的验证码</param>
        /// <param name="digits">验证码位数</param>
        /// <param name="period">验证码更新周期（秒）</param>
        /// <param name="algorithm">哈希算法（SHA1/SHA256/SHA512）</param>
        /// <param name="window">验证窗口（前后各验证几个时间步）</param>
        /// <returns>是否验证通过</returns>
        public static bool ValidateTotp(string secret, string code, int digits = 6, int period = 30, string algorithm = "SHA1", int window = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return false;
                
                // 解码Base32密钥
                var secretBytes = Base32Encoding.ToBytes(secret);
                
                // 选择哈希算法
                OtpHashMode hashMode = GetHashMode(algorithm);
                
                // 创建TOTP实例
                var totp = new Totp(secretBytes, period, hashMode, digits);
                
                // 验证验证码（window参数指定前后各验证几个时间步）
                long timeWindowUsed;
                return totp.VerifyTotp(code, out timeWindowUsed, new VerificationWindow(window, window));
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 生成随机密钥（Base32编码）
        /// </summary>
        /// <param name="length">密钥长度（字节）</param>
        /// <returns>Base32编码的密钥</returns>
        public static string GenerateRandomSecret(int length = 20)
        {
            // 使用OtpNet库生成随机密钥
            var key = KeyGeneration.GenerateRandomKey(length);
            return Base32Encoding.ToString(key);
        }

        /// <summary>
        /// 生成TOTP URI（用于生成二维码）
        /// </summary>
        /// <param name="issuer">发行者</param>
        /// <param name="accountName">账户名</param>
        /// <param name="secret">密钥（Base32编码）</param>
        /// <param name="digits">验证码位数</param>
        /// <param name="period">验证码更新周期（秒）</param>
        /// <param name="algorithm">哈希算法（SHA1/SHA256/SHA512）</param>
        /// <returns>TOTP URI</returns>
        public static string GenerateTotpUri(string issuer, string accountName, string secret, int digits = 6, int period = 30, string algorithm = "SHA1")
        {
            // 对URI参数进行URL编码
            string encodedIssuer = Uri.EscapeDataString(issuer);
            string encodedAccountName = Uri.EscapeDataString(accountName);
            
            // 构建URI
            return $"otpauth://totp/{encodedIssuer}:{encodedAccountName}?secret={secret}&issuer={encodedIssuer}&algorithm={algorithm}&digits={digits}&period={period}";
        }

        /// <summary>
        /// 根据算法名称获取OtpHashMode
        /// </summary>
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