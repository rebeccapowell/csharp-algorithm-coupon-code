// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecureRandom.cs" company="Powells IT">
//   (c) 2015 Ben Powell.
//   Credit: http://stackoverflow.com/a/16907577/119624
//   Eldar Agalarov: http://stackoverflow.com/users/2449744/eldar-agalarov
// </copyright>
// <summary>
//   The secure random class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CouponCode
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// The secure random.
    /// </summary>
    public class SecureRandom : RandomNumberGenerator
    {
        /// <summary>
        /// The random number generator.
        /// </summary>
        private readonly RandomNumberGenerator rng = new RNGCryptoServiceProvider();

        /// <summary>
        /// The next.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Next()
        {
            var data = new byte[sizeof(int)];
            this.rng.GetBytes(data);
            return BitConverter.ToInt32(data, 0) & (int.MaxValue - 1);
        }

        /// <summary>
        /// The next.
        /// </summary>
        /// <param name="maxValue">
        /// The max value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Next(int maxValue)
        {
            return this.Next(0, maxValue);
        }

        /// <summary>
        /// The next.
        /// </summary>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <returns>
        /// The <see cref="int" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">minValue cannot be greater than maxValue</exception>
        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException("minValue", minValue, "minValue cannot be greater than maxValue");
            }

            return (int)Math.Floor((minValue + ((double)maxValue) - minValue) * this.NextDouble());
        }

        /// <summary>
        /// The next double.
        /// </summary>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double NextDouble()
        {
            var data = new byte[sizeof(uint)];
            this.rng.GetBytes(data);
            var randUint = BitConverter.ToUInt32(data, 0);
            return randUint / (uint.MaxValue + 1.0);
        }

        /// <summary>
        /// The get bytes.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public override void GetBytes(byte[] data)
        {
            this.rng.GetBytes(data);
        }

        /// <summary>
        /// The get non zero bytes.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public override void GetNonZeroBytes(byte[] data)
        {
            this.rng.GetNonZeroBytes(data);
        }

        /// <summary>
        /// The get unique key.
        /// </summary>
        /// <param name="maxSize">
        /// The max size.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        
    }
}