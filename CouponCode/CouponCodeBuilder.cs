// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CouponCodeBuilder.cs" company="Powells IT">
//   Overview       : An implementation of Perl's Algorithm::CouponCode for C#.
//   Copyright      : (c) 2015 Ben Powell. 
//   License        : http://opensource.org/licenses/MIT
//
//   Ported from CPAN Algorithm-CouponCode-1.004
//      Copyright 2011 Grant McLean grantm@cpan.org
//      <url>http://cpansearch.perl.org/src/GRANTM/Algorithm-CouponCode-1.004/lib/Algorithm/CouponCode.pm</url>
//      This program is free software; you can redistribute it and/or modify it
//      under the terms of either: the GNU General Public License as published
//      by the Free Software Foundation; or the Artistic License.
// </copyright>
// <summary>
//   A 'Coupon Code' is made up of letters and numbers grouped into 4 character
//   'parts'.  For example, a 3-part code might look like this:
//   
//   1K7Q-CTFM-LMTC
//
//   Coupon Codes are random codes which are easy for the recipient to type
//   accurately into a web form.  An example application might be to print a code on
//   a letter to a customer who would then enter the code as part of the
//   registration process for web access to their account.
//
//   Features of the codes that make them well suited to manual transcription:
//    - The codes are not case sensitive.
//    - Not all letters and numbers are used, so if a person enters the letter 'O' we
//      can automatically correct it to the digit '0' (similarly for I => 1, S => 5, Z => 2).
//   - The 4th character of each part is a checkdigit, so client-side scripting can
//      be used to highlight parts which have been mis-typed, before the code is even
//      submitted to the application's back-end validation.
//   - The checkdigit algorithm takes into account the position of the part being
//      keyed.  So for example '1K7Q' might be valid in the first part but not in the
//      second so if a user typed the parts in the wrong boxes then their error could
//      be highlighted.
//   - The code generation algorithm avoids 'undesirable' codes. For example any code
//      in which transposed characters happen to result in a valid checkdigit will be
//      skipped.  Any generated part which happens to spell an 'inappropriate' 4-letter
//      word (e.g.: 'P00P') will also be skipped.
//   - The code returned by Generate() is random, but not necessarily unique.
//      If your application requires unique codes, it is your responsibility to
//      avoid duplicates (for example by using a unique index on your database column).
//   - The codes are generated using a SHA1 cryptographic hash of a plaintext.  If you
//      do not supply a plaintext, one will be generated for you (using RNGCryptoServiceProvider).  
//      In the event that an 'inappropriate' code is created, the generated hash will be 
//      used as a plaintext input for generating a new hash and the process will be repeated.
//   - Each 4-character part encodes 15 bits of random data, so a 3-part code will
//      incorporate 45 bits making a total of 2^45 (approximately 35 trillion) unique
//      codes.
//
//  string Generate(Options opts) method:
//  Returns a coupon code as a string of 4-character parts separated by '-'
//  characters.  The following optional named parameters may be supplied:
//  
//  Parts
//  The number of parts desired.  Must be a number in the range 1 - 6.  Default is 3.
//  
//  Plaintext
//  A byte string which will be hashed using Digest::SHA to produce the code.
//  If you do not supply your own plaintext then a random one will be generated for you.
//
//  string Validate(string code, Options opts)
//  
//  Takes a code, cleans it up and validates the checkdigits.  Returns the
//  normalised (and untainted) version of the code on success or undef on error.
//  The following named parameters may be supplied:
//  
//  Code
//  The code to be validated.  The parameter is mandatory.
//  
//  Parts
//  The number of parts you expect the code to contain.  Default is 3.
// </summary>
// <author>
//   Ben Powell
//   <url>https://benpowell.org</url>
//   <url>https://twitter.com/junto</url>
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace CouponCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// The coupon code builder.
    /// </summary>
    public class CouponCodeBuilder
    {
        /// <summary>
        /// The symbols dictionary.
        /// </summary>
        private readonly Dictionary<char, int> symbolsDictionary = new Dictionary<char, int>();

        /// <summary>
        /// The random number generator
        /// </summary>
        private readonly RandomNumberGenerator randomNumberGenerator;

        /// <summary>
        /// The symbols array.
        /// </summary>
        private char[] symbols;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponCodeBuilder"/> class.
        /// </summary>
        public CouponCodeBuilder()
        {
            this.BadWordsList = "SHPX PHAG JNAX JNAT CVFF PBPX FUVG GJNG GVGF SNEG URYY ZHSS QVPX XABO NEFR FUNT GBFF FYHG GHEQ FYNT PENC CBBC OHGG SRPX OBBO WVFZ WVMM CUNG'".Split(' ');
            this.SetupSymbolsDictionary();
            this.randomNumberGenerator = new SecureRandom();
        }

        /// <summary>
        /// Gets or sets the bad words list.
        /// </summary>
        public string[] BadWordsList { get; set; }

        /// <summary>
        /// The generate.
        /// </summary>
        /// <param name="opts">
        /// The opts.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Generate(Options opts)
        {
            var parts = new List<string>();

            // if  plaintext wasn't set then override
            if (string.IsNullOrEmpty(opts.Plaintext))
            {
                // not yet implemented
                opts.Plaintext = this.GetRandomPlaintext(8);
            }

            // generate parts and combine
            do
            {
                for (var i = 0; i < opts.Parts; i++)
                {
                    var sb = new StringBuilder();
                    for (var j = 0; j < opts.PartLength - 1; j++)
                    {
                        sb.Append(this.GetRandomSymbol());
                    }

                    var part = sb.ToString();
                    sb.Append(this.CheckDigitAlg1(part, i + 1));
                    parts.Add(sb.ToString());
                }
            }
            while (this.ContainsBadWord(string.Join(string.Empty, parts.ToArray())));

            return string.Join("-", parts.ToArray());
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="opts">The opts.</param>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        /// <exception cref="System.Exception">Provide a code to be validated</exception>
        /// <exception cref="Exception"></exception>
        public string Validate(string code, Options opts)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new Exception("Provide a code to be validated");
            }

            // uppercase the code, replace OIZS with 0125
            code = new string(Array.FindAll(code.ToCharArray(), char.IsLetterOrDigit))
                .ToUpper()
                .Replace("O", "0")
                .Replace("I", "1")
                .Replace("Z", "2")
                .Replace("S", "5");

            // split in the different parts
            var parts = new List<string>();
            var tmp = code;
            while (tmp.Length > 0)
            {
                parts.Add(tmp.Substring(0, opts.PartLength));
                tmp = tmp.Substring(opts.PartLength);
            }

            // make sure we have been given the same number of parts as we are expecting
            if (parts.Count != opts.Parts)
            {
                return string.Empty;
            }

            // validate each part
            for (var i = 0; i < parts.Count; i++)
            {
                var part = parts[i];

                // check this part has 4 chars
                if (part.Length != opts.PartLength)
                {
                    return string.Empty;
                }

                // split out the data and the check
                var data = part.Substring(0, opts.PartLength - 1);
                var check = part.Substring(opts.PartLength - 1, 1);

                if (Convert.ToChar(check) != this.CheckDigitAlg1(data, i + 1))
                {
                    return string.Empty;
                }
            }

            // everything looked ok with this code
            return string.Join("-", parts.ToArray());
        }

        /// <summary>
        /// The get random plaintext.
        /// </summary>
        /// <param name="maxSize">
        /// The max Size.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetRandomPlaintext(int maxSize)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            var data = new byte[1];
            this.randomNumberGenerator.GetNonZeroBytes(data);

            data = new byte[maxSize];
            this.randomNumberGenerator.GetNonZeroBytes(data);

            var result = new StringBuilder(maxSize);
            foreach (var b in data)
            {
                result.Append(chars[b % chars.Length]);
            }

            return result.ToString();
        }

        /// <summary>
        /// The get random symbol.
        /// </summary>
        /// <returns>
        /// The <see cref="char"/>.
        /// </returns>
        private char GetRandomSymbol()
        {
            var rng = new SecureRandom();
            var pos = rng.Next(this.symbols.Length);
            return this.symbols[pos];
        }

        /// <summary>
        /// The check digit algorithm 1.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="check">
        /// The check.
        /// </param>
        /// <returns>
        /// The <see cref="char"/>.
        /// </returns>
        private char CheckDigitAlg1(string data, long check)
        {
            // check's initial value is the part number (e.g. 3 or above)
            // loop through the data chars
            Array.ForEach(
                data.ToCharArray(),
                v =>
                    {
                        var k = this.symbolsDictionary[v];
                        check = (check * 19) + k;
                    });

            return this.symbols[check % (this.symbols.Length -1)];
        }

        /// <summary>
        /// The contains bad word.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ContainsBadWord(string code)
        {
            return this.BadWordsList.Any(t => code.ToUpper().IndexOf(t, StringComparison.Ordinal) > -1);
        }

        /// <summary>
        /// The setup of the symbols dictionary.
        /// </summary>
        private void SetupSymbolsDictionary()
        {
            const string AvailableSymbols = "0123456789ABCDEFGHJKLMNPQRTUVWXY";
            this.symbols = AvailableSymbols.ToCharArray();
            for (var i = 0; i < this.symbols.Length; i++)
            {
                this.symbolsDictionary.Add(this.symbols[i], i);
            }
        }
    }
}
