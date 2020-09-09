using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Powell.CouponCode;

namespace Powell.CouponCodeTests
{
    /// <summary>
    /// The coupon tests.
    /// </summary>
    public class CouponCodeTests
    {
        [Test]
        public static void BadWordsListCanBeSetByFunction()
        {
            var opts = new Options
            {
                Parts = 10,
                PartLength = 5
            };
            var ccb = new CouponCodeBuilder
            {
                // delegate defined, should invoke
                // useful for setting this list using an external data source
                SetBadWordsList = () => new List<string> {"DAMN", "NSFW"}
            };

            var code = ccb.Generate(opts);
            var badWords = ccb.BadWordsList;
            var output = ccb.Validate(code, opts);

            Console.WriteLine(code);

            // check code and validation are the same
            Assert.That(badWords, Has.Member("DAMN"));
            Assert.That(badWords, Has.Member("NSFW"));
            Assert.AreEqual(code, output, "Expected test case to ensure that the generated code and validated code match.");

        }

        [Test]
        public static void BadWordsListSetAndCheckedForEmptyStrings()
        {
            var opts = new Options
            {
                Parts = 10,
                PartLength = 5
            };
            var ccb = new CouponCodeBuilder
            {
                BadWordsList = new List<string> {"DAMN", "NSFW", string.Empty}
            };

            var code = ccb.Generate(opts);
            var badWords = ccb.BadWordsList;
            var output = ccb.Validate(code, opts);

            Console.WriteLine(code);

            // check code and validation are the same
            Assert.That(badWords, Has.Member("DAMN"));
            Assert.That(badWords, Has.Member("NSFW"));
            Assert.AreEqual(code, output, "Expected test case to ensure that the generated code and validated code match.");

        }

        /// <summary>
        /// The generate valid coupon codes also validates.
        /// </summary>
        /// <param name="counter">
        /// The counter.
        /// </param>
        [Test, TestCaseSource("GenerateTestCases")]
        public void GenerateValidCouponCodesAlsoValidates(int counter)
        {
            var opts = new Options();
            var ccb = new CouponCodeBuilder();
            var badWords = ccb.BadWordsList;

            var code = ccb.Generate(opts);
            var output = ccb.Validate(code, opts);

            Console.WriteLine(code);

            // check code and validation are the same
            Assert.IsNotNull(output, string.Format("Expected test case {0} to be not null or empty.", counter));
            Assert.AreEqual(code, output, string.Format("Expected test case {0} to ensure that the generated code and validated code match.", counter));

            // assert no bad words
            var parts = output.Split('-');
            var contains = badWords.Any(part => parts.Any(item => part.ToUpperInvariant().Contains(item.ToUpperInvariant())));
            Assert.IsFalse(contains, string.Format("Expected test case {0} to contain no bad words.", counter));
        }

        /// <summary>
        /// The generate valid coupon codes also validates.
        /// </summary>
        /// <param name="counter">
        /// The counter.
        /// </param>
        [Test, TestCaseSource("GenerateTestCases")]
        public void GenerateLongerValidCouponCodesAlsoValidates(int counter)
        {
            var opts = new Options { PartLength = 10 };
            var ccb = new CouponCodeBuilder();
            var badWords = ccb.BadWordsList;

            var code = ccb.Generate(opts);
            var output = ccb.Validate(code, opts);

            Console.WriteLine(code);

            // check code and validation are the same
            Assert.IsNotNull(output, string.Format("Expected test case {0} to be not null or empty.", counter));
            Assert.AreEqual(code, output, string.Format("Expected test case {0} to ensure that the generated code and validated code match.", counter));

            // assert no bad words
            var parts = output.Split('-');
            var contains = badWords.Any(part => parts.Any(item => part.ToUpperInvariant().Contains(item.ToUpperInvariant())));
            Assert.IsFalse(contains, string.Format("Expected test case {0} to contain no bad words.", counter));
        }

        /// <summary>
        /// The validate pre-generated coupon codes has no errors.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        [Test, TestCaseSource("GenerateValidationTestCases")]
        public void ValidatePregeneratedCouponCodesHasNoErrors(string code)
        {
            var opts = new Options();
            var ccb = new CouponCodeBuilder();
            var output = ccb.Validate(code, opts);

            // check code and validation are the same
            Assert.IsNotNull(output, string.Format("Expected test case {0} to be not null or empty.", code));
            Assert.AreEqual(code, output, string.Format("Expected test case {0} to ensure that the generated code and validated code match.", code));
        }

        /// <summary>
        /// The invalid coupon code return empty string.
        /// </summary>
        [Test]
        public void InvalidCouponCodeReturnEmptyString()
        {
            var opts = new Options();
            var ccb = new CouponCodeBuilder();

            // valid is "9Y46-9M8E-UQB8"
            var output = ccb.Validate("9Y46-9M8E-UQBA", opts);

            // check output is empty
            Assert.IsEmpty(output);
        }

        /// <summary>
        /// The generate test cases.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private static IEnumerable GenerateTestCases()
        {
            for (var i = 0; i < 500; i++)
            {
                yield return new TestCaseData(i);
            }
        }

        /// <summary>
        /// Gets the generate validation test cases.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private static IEnumerable GenerateValidationTestCases()
        {
            yield return new TestCaseData("9Y46-9M8E-UQB8");
            yield return new TestCaseData("BDW8-9HD5-RBG5");
            yield return new TestCaseData("AWN6-PTW7-3275");
            yield return new TestCaseData("4J1V-RTWG-E8F6");
            yield return new TestCaseData("73F3-FEHA-1XNG");
            yield return new TestCaseData("3WQP-BHWX-HMK7");
            yield return new TestCaseData("T3MH-NG9R-GXVC");
            yield return new TestCaseData("NCMF-07XQ-76A9");
            yield return new TestCaseData("JCV4-H8GV-6WP5");
            yield return new TestCaseData("YV9N-KLKL-LX35");
            yield return new TestCaseData("JB8U-8WAQ-N9XE");
            yield return new TestCaseData("EVUA-VY5P-E2NP");
            yield return new TestCaseData("4HX7-PNYT-2GTN");
            yield return new TestCaseData("1TC7-G6VD-DAW7");
            yield return new TestCaseData("NJJ2-FV7J-305U");
            yield return new TestCaseData("J870-8PQH-E97H");
            yield return new TestCaseData("EU83-MCFV-5E8T");
            yield return new TestCaseData("6PEM-V3Q6-G7YC");
            yield return new TestCaseData("KCNJ-FD38-28BA");
            yield return new TestCaseData("0Y9H-2AA8-LUBJ");
            yield return new TestCaseData("4A43-KU4E-NE9T");
            yield return new TestCaseData("2QQ1-PVCV-155L");
            yield return new TestCaseData("5H1W-Y6JQ-CEAD");
            yield return new TestCaseData("G0UE-DKK5-9P0M");
            yield return new TestCaseData("LLP5-2L9B-QV1E");
            yield return new TestCaseData("5AH5-MU0K-C26W");
            yield return new TestCaseData("XGL2-4BLF-YA52");
            yield return new TestCaseData("HDGN-VW6H-CEJM");
            yield return new TestCaseData("C1AW-F43P-75N2");
            yield return new TestCaseData("CNL4-WAT6-66DP");
            yield return new TestCaseData("2W3D-CBDD-91U2");
            yield return new TestCaseData("FLEL-74XE-8Q2N");
            yield return new TestCaseData("GPD3-1KA4-1TGU");
            yield return new TestCaseData("DW2F-9E96-39BJ");
            yield return new TestCaseData("EYFQ-UK6Q-02XX");
            yield return new TestCaseData("CPPT-Y56Q-XBVQ");
            yield return new TestCaseData("886G-48QQ-4DFR");
            yield return new TestCaseData("5M3E-5FHF-9CUR");
            yield return new TestCaseData("EV2G-6PP7-RWXL");
            yield return new TestCaseData("GHEE-E0UD-L2QM");
            yield return new TestCaseData("UQET-U40C-83C5");
            yield return new TestCaseData("HHPC-BFTL-QE1U");
            yield return new TestCaseData("5PE1-86M1-B7BG");
            yield return new TestCaseData("1H5F-XBF3-4CJ9");
            yield return new TestCaseData("WV7B-PKY0-TGFT");
            yield return new TestCaseData("8YQ6-N7UU-TV6V");
            yield return new TestCaseData("KX0V-XHEP-3CY2");
            yield return new TestCaseData("EFYF-29V7-E35R");
            yield return new TestCaseData("0RL7-VKEM-833U");
            yield return new TestCaseData("0U2U-D16N-AXJ6");
            yield return new TestCaseData("A7RR-X575-0BTB");
            yield return new TestCaseData("LYT0-BX22-CYEX");
            yield return new TestCaseData("BG49-CGMP-JFG3");
            yield return new TestCaseData("9CBQ-PQX1-QY7F");
            yield return new TestCaseData("6MUU-C03B-AUB4");
            yield return new TestCaseData("PXWD-DF14-8GXN");
            yield return new TestCaseData("9QTK-AMVP-6L85");
            yield return new TestCaseData("3UA2-7BG9-LTJ6");
            yield return new TestCaseData("4EEU-HC72-0C04");
            yield return new TestCaseData("LW31-PQPR-1TN2");
            yield return new TestCaseData("GJXJ-1WCA-56D3");
            yield return new TestCaseData("TLBL-KVVT-T1GM");
            yield return new TestCaseData("4P0W-EUTW-32CA");
            yield return new TestCaseData("PJX3-WBFE-FMV7");
            yield return new TestCaseData("DV71-8MWF-XDUX");
            yield return new TestCaseData("L8PR-EJBW-PLNJ");
            yield return new TestCaseData("2Q08-8XJL-NC3D");
            yield return new TestCaseData("3A5F-13WW-7C8V");
            yield return new TestCaseData("R3FN-E3V9-7DY8");
            yield return new TestCaseData("Q87U-W8JN-1GR1");
            yield return new TestCaseData("XHAB-5G2K-RPMM");
            yield return new TestCaseData("JNP3-MMCA-WLWM");
            yield return new TestCaseData("D0PC-8QK0-YJ2U");
            yield return new TestCaseData("3RRA-6AYG-6EJR");
            yield return new TestCaseData("WK1L-QY00-T292");
            yield return new TestCaseData("8HMG-3N86-NVCG");
            yield return new TestCaseData("M573-D9P5-2B4W");
            yield return new TestCaseData("N75V-22JK-22BL");
            yield return new TestCaseData("2400-2BUD-D1RJ");
            yield return new TestCaseData("CKVH-VMWC-HH49");
            yield return new TestCaseData("YCBX-3ERT-4C5U");
            yield return new TestCaseData("ENWN-AJY0-R3F7");
            yield return new TestCaseData("VT8G-14QC-PR86");
            yield return new TestCaseData("6AAJ-WE4W-UKPJ");
            yield return new TestCaseData("4N4E-4XWD-3XMQ");
            yield return new TestCaseData("7MAX-WC6Q-BQE1");
            yield return new TestCaseData("UFA6-HFG6-TVH8");
            yield return new TestCaseData("YA4G-GDF9-3RY1");
            yield return new TestCaseData("3VAM-J45P-EME4");
            yield return new TestCaseData("U9FM-NDMB-5UL6");
            yield return new TestCaseData("RT6G-RAC5-QJ1A");
            yield return new TestCaseData("P1C3-L123-W8T7");
            yield return new TestCaseData("T9K5-UARU-REWD");
            yield return new TestCaseData("B0KX-MTLL-FHFB");
            yield return new TestCaseData("46G1-BVB4-KT43");
            yield return new TestCaseData("3QVR-77HU-0BDW");
            yield return new TestCaseData("M01T-EYN8-NAMQ");
            yield return new TestCaseData("NV2M-A4GW-5KYL");
            yield return new TestCaseData("VL4N-TY3C-GGQM");
            yield return new TestCaseData("B5HX-B0M9-K2FP");
            yield return new TestCaseData("BQ9B-W6WT-NHL1");
            yield return new TestCaseData("4UET-LKVX-26UK");
            yield return new TestCaseData("68KL-6EML-031L");
            yield return new TestCaseData("Y2R9-ARV6-CDN6");
            yield return new TestCaseData("X1PX-1UB2-BQM8");
            yield return new TestCaseData("96KB-65P6-TQAJ");
            yield return new TestCaseData("NAD0-R0UG-L3L5");
            yield return new TestCaseData("A2WU-CC2M-6N04");
            yield return new TestCaseData("EPHW-4UD2-NNYE");
            yield return new TestCaseData("98YX-JDTW-REBT");
            yield return new TestCaseData("0490-79YH-RXKV");
            yield return new TestCaseData("D5F6-19PD-LARK");
            yield return new TestCaseData("UL0W-VLQK-XL1D");
            yield return new TestCaseData("L1QH-90AL-EPFC");
            yield return new TestCaseData("N6FK-7MHE-277J");
            yield return new TestCaseData("30JQ-XDA5-UA5F");
            yield return new TestCaseData("GVPF-7BYQ-C71T");
            yield return new TestCaseData("EAEU-MDMN-YPLG");
            yield return new TestCaseData("7GL7-LL3Q-V4P1");
            yield return new TestCaseData("6A4C-HC1U-5UWF");
            yield return new TestCaseData("V0V7-MDTU-X7FV");
            yield return new TestCaseData("0B66-TK0W-CLUL");
            yield return new TestCaseData("K5AV-W5W7-MHC4");
            yield return new TestCaseData("YJGR-GJLG-9RFC");
            yield return new TestCaseData("GJWH-3626-E87W");
            yield return new TestCaseData("5W5D-XHP1-6WT8");
            yield return new TestCaseData("FCP1-HHC9-AHG5");
            yield return new TestCaseData("KVE4-5H9E-VA65");
            yield return new TestCaseData("Q6Q6-7U0J-NRN0");
            yield return new TestCaseData("8YT8-TGCF-NPX1");
            yield return new TestCaseData("TJKM-9JDQ-JL9W");
            yield return new TestCaseData("E6NM-AGUL-CXUQ");
            yield return new TestCaseData("8GR1-CH1N-35XP");
            yield return new TestCaseData("NVWH-8FA6-L8L7");
            yield return new TestCaseData("CM95-EWV7-923V");
            yield return new TestCaseData("PYKN-7BXP-5Q0N");
            yield return new TestCaseData("7A41-8A93-2BC6");
            yield return new TestCaseData("U2B8-0Y7P-L6T6");
            yield return new TestCaseData("WXT6-YPQC-UMW0");
            yield return new TestCaseData("12RW-HDAQ-8YNL");
            yield return new TestCaseData("HJQ1-P89H-EB9T");
            yield return new TestCaseData("370F-JFCN-7LG2");
            yield return new TestCaseData("KUCE-WJR2-UE5W");
            yield return new TestCaseData("T6J9-82EB-N0FE");
            yield return new TestCaseData("75E9-4YBE-B7W3");
            yield return new TestCaseData("463K-N0E5-LQ5H");
            yield return new TestCaseData("MVTR-E2D6-6YVH");
            yield return new TestCaseData("H3NQ-J7WB-5ALQ");
            yield return new TestCaseData("2HNM-0V3Q-TYJ4");
            yield return new TestCaseData("8RE6-8PNF-D7R8");
            yield return new TestCaseData("YFN5-8XX1-VP1X");
            yield return new TestCaseData("AEV6-RFE9-UEPG");
            yield return new TestCaseData("1YDA-LVAV-TJPA");
            yield return new TestCaseData("VJXA-R67H-UJV4");
            yield return new TestCaseData("378P-64E9-LN49");
            yield return new TestCaseData("XH34-YJP9-R5KJ");
            yield return new TestCaseData("QKL1-YWCM-X5X5");
            yield return new TestCaseData("W4AP-2H4B-AQ75");
            yield return new TestCaseData("H7WE-L1LM-1RWM");
            yield return new TestCaseData("6F6G-R88R-DT9C");
            yield return new TestCaseData("N25T-U8NH-ENJU");
            yield return new TestCaseData("VKHG-AC9K-F7XN");
            yield return new TestCaseData("JR6C-2EBP-MKWV");
            yield return new TestCaseData("6VDN-NMJ5-XKYQ");
            yield return new TestCaseData("D5B2-NKM1-TR1V");
            yield return new TestCaseData("YVP5-VG5H-49XT");
            yield return new TestCaseData("VWUX-DT61-B6P9");
            yield return new TestCaseData("6682-2MRF-7JW8");
            yield return new TestCaseData("16BW-57PQ-JWEK");
            yield return new TestCaseData("RH71-6WV2-AQ42");
            yield return new TestCaseData("WGA3-AJ11-1HTM");
            yield return new TestCaseData("L5EM-TL4M-8DB8");
            yield return new TestCaseData("VFK4-0X63-L0J8");
            yield return new TestCaseData("67N4-7E0K-XUG6");
            yield return new TestCaseData("VBUW-9MQX-DPNX");
            yield return new TestCaseData("T3C8-G2MP-T794");
            yield return new TestCaseData("QFPM-TQDD-8YJG");
            yield return new TestCaseData("9J9C-4FEP-0RMQ");
            yield return new TestCaseData("VQ67-N427-GEK9");
            yield return new TestCaseData("JGC2-8KU6-X1RH");
            yield return new TestCaseData("C5DF-YVB1-RHCN");
            yield return new TestCaseData("XP5U-MFKU-VWNA");
            yield return new TestCaseData("RYDR-V1NV-AGCD");
            yield return new TestCaseData("K50J-58WJ-QRF2");
            yield return new TestCaseData("LE0P-P4WP-XL0C");
            yield return new TestCaseData("NEEF-GPMK-58UQ");
            yield return new TestCaseData("Q9V5-31PR-HYLC");
            yield return new TestCaseData("MUAM-DW7V-UM02");
            yield return new TestCaseData("14R5-UTHD-A71H");
            yield return new TestCaseData("XKV5-TN0Q-WDVB");
            yield return new TestCaseData("R8BL-AMFA-EJMG");
            yield return new TestCaseData("TWLE-5UEP-U869");
            yield return new TestCaseData("T3UP-YWKV-T81F");
            yield return new TestCaseData("LWFD-U62M-9CXV");
            yield return new TestCaseData("80HX-C631-AAR5");
            yield return new TestCaseData("J35U-4DNQ-PHAB");
            yield return new TestCaseData("G53P-NPD7-UUJA");
            yield return new TestCaseData("0TFM-WXK7-YAA7");
            yield return new TestCaseData("6FX9-3E78-7H1P");
            yield return new TestCaseData("7QM5-TL2K-1XTL");
            yield return new TestCaseData("71XB-81CM-WNRQ");
            yield return new TestCaseData("8VQB-1W75-47B0");
            yield return new TestCaseData("K1EJ-E2RJ-53B6");
            yield return new TestCaseData("C1F3-FN1N-P04P");
            yield return new TestCaseData("20BV-DUQ7-CGAL");
            yield return new TestCaseData("Q287-UHER-AXPB");
            yield return new TestCaseData("MC7C-LQGL-YP1V");
            yield return new TestCaseData("2BKV-971L-18BM");
            yield return new TestCaseData("T66V-NT6T-P6Y9");
            yield return new TestCaseData("GH11-6PQ8-P7RN");
            yield return new TestCaseData("QYVL-BAVL-R576");
            yield return new TestCaseData("X6KV-CJR3-AQKH");
            yield return new TestCaseData("MHR1-MVCK-PFHB");
            yield return new TestCaseData("TC1D-Q3YT-PYJ6");
            yield return new TestCaseData("3JPX-0C2W-0JPH");
            yield return new TestCaseData("6G64-KGWG-WFTG");
            yield return new TestCaseData("UX24-D36W-RWWK");
            yield return new TestCaseData("CE5P-N18J-YNR2");
            yield return new TestCaseData("TWNG-LDFU-771K");
            yield return new TestCaseData("715H-NFPL-8UXE");
            yield return new TestCaseData("AHW2-018C-BUAP");
            yield return new TestCaseData("TRDQ-N0B2-YK7L");
            yield return new TestCaseData("8E88-BX55-G855");
            yield return new TestCaseData("XQDP-LEXX-TPYL");
            yield return new TestCaseData("JL6A-6UVT-THPN");
            yield return new TestCaseData("6YAE-UQG5-PGM3");
            yield return new TestCaseData("DWN4-8M7Q-KXYD");
            yield return new TestCaseData("LVQ3-ABE5-0X49");
            yield return new TestCaseData("PQ2U-8Q8L-CMWA");
            yield return new TestCaseData("RDGU-89T1-408K");
            yield return new TestCaseData("9RK0-99KE-77XH");
            yield return new TestCaseData("D9HN-7MPL-YP40");
            yield return new TestCaseData("7XCH-M3RN-UNNC");
            yield return new TestCaseData("TW3V-LTAM-M7M9");
            yield return new TestCaseData("V7PB-56R7-U4VH");
            yield return new TestCaseData("J42C-KWK5-AQLJ");
            yield return new TestCaseData("3TDH-A44H-MEKG");
            yield return new TestCaseData("ARAB-DE7N-LUV4");
            yield return new TestCaseData("A090-UCKV-DF5G");
            yield return new TestCaseData("CXV9-BL51-K7Q3");
            yield return new TestCaseData("U54U-VBLX-RN2E");
            yield return new TestCaseData("81QR-XHR3-JHGA");
            yield return new TestCaseData("4GC1-HW4C-L2KG");
            yield return new TestCaseData("LFL0-YRE9-WTU9");
            yield return new TestCaseData("Q5UM-7P9D-Q08G");
            yield return new TestCaseData("2B1A-WQ75-JDW9");
            yield return new TestCaseData("XTE0-8JLB-WMM1");
            yield return new TestCaseData("JWMA-P3HP-9NFH");
            yield return new TestCaseData("BTGR-QR3D-8LMU");
            yield return new TestCaseData("T2W6-2EM2-78GN");
            yield return new TestCaseData("7PYU-JMUU-L2MJ");
            yield return new TestCaseData("0QBA-ANH0-T477");
            yield return new TestCaseData("004C-Y2G8-XFP2");
            yield return new TestCaseData("BCA1-8DWJ-HDA1");
            yield return new TestCaseData("UK8J-PA1G-BE3H");
            yield return new TestCaseData("L7R8-9X7W-ATNU");
            yield return new TestCaseData("2KY6-RD7T-CV3Q");
            yield return new TestCaseData("YPV8-X6L6-V6Q9");
            yield return new TestCaseData("4V77-GUET-TRD9");
            yield return new TestCaseData("9VKT-6URP-CL81");
            yield return new TestCaseData("TLT4-HBPX-K5TW");
            yield return new TestCaseData("VEA7-G1EV-URWE");
            yield return new TestCaseData("FQVH-798R-85KK");
            yield return new TestCaseData("PG30-FQ1W-GKVL");
            yield return new TestCaseData("27WQ-RPE6-2DQR");
            yield return new TestCaseData("JPDC-PV0G-8ADF");
            yield return new TestCaseData("15UT-L75U-9NAC");
            yield return new TestCaseData("G2PH-VQ6F-8CAK");
            yield return new TestCaseData("3NF5-B16D-6D7T");
            yield return new TestCaseData("C11L-YFF6-QVFV");
            yield return new TestCaseData("38HL-LX2U-78QX");
            yield return new TestCaseData("6WDA-PHB4-382M");
            yield return new TestCaseData("Y9NF-P4JC-C4RQ");
            yield return new TestCaseData("YGTV-UKH4-YXQW");
            yield return new TestCaseData("WCGT-WW99-GNPA");
            yield return new TestCaseData("917V-CL7P-LTM9");
            yield return new TestCaseData("Q55X-QYRR-4L7T");
            yield return new TestCaseData("D0G5-0JDX-RB0L");
            yield return new TestCaseData("R9WT-9RAX-NKE2");
            yield return new TestCaseData("FQH6-J048-8E7P");
            yield return new TestCaseData("06XV-H5YH-4CTH");
            yield return new TestCaseData("K901-EFA2-HMVG");
            yield return new TestCaseData("3C6P-CXJ7-QWFG");
            yield return new TestCaseData("GFLD-GE5J-72N7");
            yield return new TestCaseData("1RKT-XF24-6NQV");
            yield return new TestCaseData("DKFQ-W6JF-1K57");
            yield return new TestCaseData("1PLL-HWQ1-CEX2");
            yield return new TestCaseData("VK10-43MK-VWXJ");
            yield return new TestCaseData("G3WB-DF8B-CM9M");
            yield return new TestCaseData("50CU-NKT6-MVK3");
            yield return new TestCaseData("N0AQ-HYDV-AGX0");
            yield return new TestCaseData("95TX-YM2E-JKDE");
            yield return new TestCaseData("M9FR-CTCJ-TTH1");
            yield return new TestCaseData("UJ4T-FX7R-0W5N");
            yield return new TestCaseData("HWAA-8W0E-C0DW");
            yield return new TestCaseData("W6G5-RCBB-KE3N");
            yield return new TestCaseData("0P5G-EL60-T618");
            yield return new TestCaseData("4CNV-PFDX-D5X6");
            yield return new TestCaseData("Q254-C85A-BNDQ");
            yield return new TestCaseData("VMBH-83DW-PVTK");
            yield return new TestCaseData("D8AU-69VR-PK8G");
            yield return new TestCaseData("XRVT-4F2B-45YD");
            yield return new TestCaseData("9CHX-7UVF-8QWJ");
            yield return new TestCaseData("DQT6-FKE9-0YVM");
            yield return new TestCaseData("Q593-U9FW-L8N9");
            yield return new TestCaseData("AV73-5T5T-XV9J");
            yield return new TestCaseData("N7MD-0XQM-WEUW");
            yield return new TestCaseData("37DV-HG4D-RP55");
            yield return new TestCaseData("0VM3-10BG-PBXA");
            yield return new TestCaseData("QTQE-8AA4-3TB0");
            yield return new TestCaseData("5DFW-DU4J-M611");
            yield return new TestCaseData("L4QC-1J28-G386");
            yield return new TestCaseData("WDVT-Y0J3-PU38");
            yield return new TestCaseData("WL8F-HR3V-9N57");
            yield return new TestCaseData("LXG2-B59X-4GBG");
            yield return new TestCaseData("22NF-QGKD-16AD");
            yield return new TestCaseData("W47L-F5NX-5N1G");
            yield return new TestCaseData("DW3G-JEND-4PXD");
            yield return new TestCaseData("FNXC-FBKH-RXR3");
            yield return new TestCaseData("1532-98R1-CLJB");
            yield return new TestCaseData("YLN7-QTVT-AL2H");
            yield return new TestCaseData("FUAR-1366-N0WV");
            yield return new TestCaseData("F718-16B6-TMEU");
            yield return new TestCaseData("8GV4-JUA0-MHUK");
            yield return new TestCaseData("AXQU-63T2-EQYG");
            yield return new TestCaseData("VLQB-H5H3-G6UL");
            yield return new TestCaseData("9GQL-3XPJ-8CDN");
            yield return new TestCaseData("3L7M-VG3F-F0WC");
            yield return new TestCaseData("TAMT-4T45-79NG");
            yield return new TestCaseData("XLB7-Q0PP-3CKM");
            yield return new TestCaseData("UGTA-FH5Q-0C59");
            yield return new TestCaseData("RVTC-6GGN-04R1");
            yield return new TestCaseData("C1G4-A0FE-TUTW");
            yield return new TestCaseData("P74G-E0WF-UHF3");
            yield return new TestCaseData("EACR-7JMP-CRXR");
            yield return new TestCaseData("39XM-62XJ-C6LT");
            yield return new TestCaseData("T525-KPD9-JLWJ");
            yield return new TestCaseData("HAR5-14L8-B0QL");
            yield return new TestCaseData("G54Q-BUX4-MQKL");
            yield return new TestCaseData("QFB9-QC2D-0B9R");
            yield return new TestCaseData("U56W-PACU-ALQ8");
            yield return new TestCaseData("9GUP-TNME-HVDA");
            yield return new TestCaseData("40UN-UJVU-BW7U");
            yield return new TestCaseData("MBXG-FNB1-PFA4");
            yield return new TestCaseData("9024-0A7U-G0GK");
            yield return new TestCaseData("NQGM-FDFL-3QEU");
            yield return new TestCaseData("6TJL-U729-UD7C");
            yield return new TestCaseData("JYMH-E1W3-LJND");
            yield return new TestCaseData("4F23-ACEQ-WQPW");
            yield return new TestCaseData("DE8F-2UM1-VWYK");
            yield return new TestCaseData("JHDN-VX10-2H0F");
            yield return new TestCaseData("UFKF-4V08-FQ16");
            yield return new TestCaseData("T61P-1FP3-5M95");
            yield return new TestCaseData("5RC6-RYAX-MGPU");
            yield return new TestCaseData("9GHD-B8P8-W7QH");
            yield return new TestCaseData("HV1D-DQ6R-JMKU");
            yield return new TestCaseData("2F80-U9WC-YG4N");
            yield return new TestCaseData("DGP6-BNDG-NL8F");
            yield return new TestCaseData("X1Y7-M18W-5PKN");
            yield return new TestCaseData("0A0C-N9X6-5GLE");
            yield return new TestCaseData("0AL1-N9DL-E0JC");
            yield return new TestCaseData("9GPK-HEY2-YDE6");
            yield return new TestCaseData("DQ4F-L7F6-0EBN");
            yield return new TestCaseData("RR90-J9D2-KLR3");
            yield return new TestCaseData("WD42-7QYP-2J69");
            yield return new TestCaseData("6K81-VMDU-BVKL");
            yield return new TestCaseData("5EY2-QRGT-BFJL");
            yield return new TestCaseData("YNE6-XBYK-XF2C");
            yield return new TestCaseData("D66G-4RGW-2EWJ");
            yield return new TestCaseData("DUMU-FG88-K23B");
            yield return new TestCaseData("UXVX-7M52-5T20");
            yield return new TestCaseData("3NTG-N907-TNNP");
            yield return new TestCaseData("V26P-L8T5-GYTW");
            yield return new TestCaseData("ENE7-VTWE-PJYL");
            yield return new TestCaseData("XNMQ-LLH7-0J93");
            yield return new TestCaseData("69NB-WJU4-QV6K");
            yield return new TestCaseData("XA0P-BY7T-1AXG");
            yield return new TestCaseData("LTPT-APKM-K8HF");
            yield return new TestCaseData("G4VW-4MLK-K8HF");
            yield return new TestCaseData("0FM4-758B-8PHJ");
            yield return new TestCaseData("0JAK-5UHT-RGWL");
            yield return new TestCaseData("BDDP-V20R-B4M0");
            yield return new TestCaseData("TJVX-05XH-FKV0");
            yield return new TestCaseData("HT9E-DBNB-NCV7");
            yield return new TestCaseData("X294-EYH3-MPM3");
            yield return new TestCaseData("5L3T-BE9F-GRHX");
            yield return new TestCaseData("L54B-TBAB-2AT1");
            yield return new TestCaseData("DDC0-8C78-D4NA");
            yield return new TestCaseData("EVP6-Y2A2-PHEF");
            yield return new TestCaseData("B4LE-PXC4-K1AX");
            yield return new TestCaseData("DR87-7F5C-VBYJ");
            yield return new TestCaseData("EMPV-775F-6MTB");
            yield return new TestCaseData("TTQP-JTMP-9N13");
            yield return new TestCaseData("TEVG-6KNP-DDDH");
            yield return new TestCaseData("VRRE-DPY0-1MT4");
            yield return new TestCaseData("59QQ-80MB-QBUU");
            yield return new TestCaseData("C8HE-G1N5-3NAG");
            yield return new TestCaseData("B9MH-05D0-FJP7");
            yield return new TestCaseData("2J1K-MUPB-15G0");
            yield return new TestCaseData("EPM2-F7R9-3PPH");
            yield return new TestCaseData("UA2U-U7GP-7HMC");
            yield return new TestCaseData("BEA8-LJ5K-9LGB");
            yield return new TestCaseData("DPXN-C7M7-F9DC");
            yield return new TestCaseData("6F6G-C11V-PM9Q");
            yield return new TestCaseData("U2QM-TNUL-B7JP");
            yield return new TestCaseData("G3DT-8WN5-J111");
            yield return new TestCaseData("MV65-F1RK-8RX7");
            yield return new TestCaseData("0VVA-EY9T-7TR1");
            yield return new TestCaseData("64E1-C7E0-B2Y3");
            yield return new TestCaseData("QQAQ-986D-N3LE");
            yield return new TestCaseData("9TPP-QEH4-R0DA");
            yield return new TestCaseData("B71M-R9EK-AB10");
            yield return new TestCaseData("URUU-B1LU-32CA");
            yield return new TestCaseData("BV6N-M8WV-1R0P");
            yield return new TestCaseData("71R6-GKK3-CGQ3");
            yield return new TestCaseData("0YW6-3UPP-JRYN");
            yield return new TestCaseData("30R0-BT4M-W1UX");
            yield return new TestCaseData("TK7V-B2YT-A2VB");
            yield return new TestCaseData("QC58-0LJB-QQ54");
            yield return new TestCaseData("2UPT-JQWQ-EHKT");
            yield return new TestCaseData("EMHN-36GL-C0N7");
            yield return new TestCaseData("FDC9-19RF-FBLT");
            yield return new TestCaseData("QN6D-0MQ5-V1FW");
            yield return new TestCaseData("4RCH-B49B-VL7A");
            yield return new TestCaseData("YD29-DCLV-YBTB");
            yield return new TestCaseData("W9EW-52ED-Q92T");
            yield return new TestCaseData("6U5T-7KB1-7A2F");
            yield return new TestCaseData("U4CG-9XMC-YWK5");
            yield return new TestCaseData("2WHU-UPFG-D12T");
            yield return new TestCaseData("KAWJ-FUF7-KHM4");
            yield return new TestCaseData("EYV6-RE4B-K0DE");
            yield return new TestCaseData("5AH5-KLHJ-HQ0E");
            yield return new TestCaseData("P7ET-GTUL-Q92T");
            yield return new TestCaseData("DNU0-FWED-X3QP");
            yield return new TestCaseData("PNTD-K7RU-B597");
            yield return new TestCaseData("G62A-QM2W-02UU");
            yield return new TestCaseData("W73B-NQFV-GEH7");
            yield return new TestCaseData("7P62-R95A-7FAR");
            yield return new TestCaseData("NEDE-W3DF-23KG");
            yield return new TestCaseData("MVXW-H7RJ-84T7");
            yield return new TestCaseData("DCDD-B35K-JTHU");
            yield return new TestCaseData("DQ9L-H0FX-0DNE");
            yield return new TestCaseData("DDWH-A9M5-2UM9");
            yield return new TestCaseData("HDPW-K8A0-L3DW");
            yield return new TestCaseData("CWXP-Y7PH-9L50");
            yield return new TestCaseData("GRVR-0WMX-W1FJ");
            yield return new TestCaseData("AN7D-P8X7-RNL1");
            yield return new TestCaseData("LUWL-YTM4-21YM");
            yield return new TestCaseData("0PU7-7864-E7GK");
            yield return new TestCaseData("4J40-8J5U-CHTQ");
            yield return new TestCaseData("L60T-3Q05-MF5M");
            yield return new TestCaseData("N5CV-7AEK-YQ4K");
            yield return new TestCaseData("7UN1-WGEF-XT24");
            yield return new TestCaseData("673G-AJFF-L9QX");
            yield return new TestCaseData("27PJ-THPE-1VG3");
            yield return new TestCaseData("8D9M-7QH9-2U4P");
            yield return new TestCaseData("ACAC-HN0X-GJW2");
            yield return new TestCaseData("MTMD-KR36-HAMH");
            yield return new TestCaseData("E2VD-PP0E-CYDW");
            yield return new TestCaseData("YWMN-H6CH-FUKK");
            yield return new TestCaseData("L322-V9V0-0Y70");
            yield return new TestCaseData("KLF8-T8T1-GMCB");
            yield return new TestCaseData("Q1N2-UTEA-Q43R");
            yield return new TestCaseData("DHEG-T8DK-UG22");
            yield return new TestCaseData("W6NB-0RFA-RYGD");
            yield return new TestCaseData("3836-82B8-JAG1");
            yield return new TestCaseData("5RE8-4GXU-K5QU");
            yield return new TestCaseData("RWLR-7BVM-J74R");
            yield return new TestCaseData("23FU-PR3Q-4MU3");
            yield return new TestCaseData("RJ5J-KUDP-8G3T");
            yield return new TestCaseData("P7AN-3TQ5-UEPG");
            yield return new TestCaseData("6VJU-HDL3-A75M");
            yield return new TestCaseData("JHY9-TX1N-H9MW");
            yield return new TestCaseData("02BT-HYWD-20LN");
            yield return new TestCaseData("7Q6M-1KE8-QCYK");
            yield return new TestCaseData("PKH9-T4NE-GQRK");
            yield return new TestCaseData("3Q52-KCGL-WQW4");
            yield return new TestCaseData("HNQF-H4CA-3HGL");
            yield return new TestCaseData("AVWR-X0LG-GLFT");
            yield return new TestCaseData("LNYL-F0NV-ECDJ");
            yield return new TestCaseData("MB4M-DQRD-CWEP");
            yield return new TestCaseData("0A9M-6RQF-KBXP");
            yield return new TestCaseData("464L-KBK4-QF6L");
            yield return new TestCaseData("41BR-6MX7-HBH1");
            yield return new TestCaseData("3Y28-J4N9-GLM1");
            yield return new TestCaseData("8W8E-AKPB-ECMT");
            yield return new TestCaseData("4TWN-0PYK-C22R");
            yield return new TestCaseData("KYH2-R50N-492W");
            yield return new TestCaseData("C8UQ-PNRL-6N04");
            yield return new TestCaseData("7233-BF82-2BE8");
            yield return new TestCaseData("M66M-W7L5-LF95");
            yield return new TestCaseData("XYH5-JKVM-C8EU");
            yield return new TestCaseData("2F1Q-D3KB-C09R");
            yield return new TestCaseData("A3H3-FCF1-7F8P");
            yield return new TestCaseData("9YAC-41LB-5MYU");
            yield return new TestCaseData("MLX1-Y0P8-092B");
            yield return new TestCaseData("0Q43-QYNN-QJCM");
            yield return new TestCaseData("DBET-ELXQ-DHTD");
            yield return new TestCaseData("AXHL-14AW-MR7U");
            yield return new TestCaseData("1074-56K1-7L2K");
            yield return new TestCaseData("90HK-8AB5-69FL");
            yield return new TestCaseData("FHYB-UA79-13NX");
            yield return new TestCaseData("N452-BQDP-2YW0");
            yield return new TestCaseData("4PA8-PGBG-WFA0");
            yield return new TestCaseData("V9MG-07RK-VV11");
            yield return new TestCaseData("AY3R-KHY6-40XA");
            yield return new TestCaseData("PB2V-2F44-M4RJ");
            yield return new TestCaseData("W2GN-32G6-VKAR");
            yield return new TestCaseData("LGQP-91DB-GX0F");
            yield return new TestCaseData("99J5-UFU0-BRFM");
            yield return new TestCaseData("EYGR-38L0-TXNL");
            yield return new TestCaseData("9GTN-CED8-2RL1");
            yield return new TestCaseData("9CGW-URAJ-2T88");
            yield return new TestCaseData("ANR0-NVB7-KL2B");
            yield return new TestCaseData("N604-HGJU-X6TL");
            yield return new TestCaseData("F6XJ-4353-84N3");
            yield return new TestCaseData("8VN9-UTRM-F987");
            yield return new TestCaseData("G3CR-PVK4-1123");
            yield return new TestCaseData("PD02-HUDE-526D");
            yield return new TestCaseData("LQ84-5EW8-Q7AU");
            yield return new TestCaseData("4KCU-GC8E-HMBX");
            yield return new TestCaseData("3JQ0-G1R8-1DYC");
            yield return new TestCaseData("1JA8-5D1P-7687");
            yield return new TestCaseData("6UYM-0TFW-W3YA");
            yield return new TestCaseData("KUTV-R5MC-D7H0");
            yield return new TestCaseData("D1AJ-FT59-8Y97");
            yield return new TestCaseData("GB6G-1ERH-EE8L");
            yield return new TestCaseData("DF94-BU16-V0WQ");
            yield return new TestCaseData("B3HP-2E6J-VJGC");
            yield return new TestCaseData("H9LC-19NC-J3GP");
            yield return new TestCaseData("2XJG-L5UB-1Q37");
            yield return new TestCaseData("FK2L-1D15-3TNB");
            yield return new TestCaseData("JU7L-KR8B-TUY3");
            yield return new TestCaseData("GVB3-X97K-JV7Q");
            yield return new TestCaseData("53M0-8XHK-BU1E");
            yield return new TestCaseData("96NE-5W2J-YU0A");
            yield return new TestCaseData("XR0W-GVY0-2A5B");
            yield return new TestCaseData("U939-BKMW-190W");
            yield return new TestCaseData("6CTA-T4JA-N41E");
            yield return new TestCaseData("575W-3PM7-QFCT");
            yield return new TestCaseData("UM5N-7YLM-ARVE");
            yield return new TestCaseData("L5MV-DCR2-XHL6");
            yield return new TestCaseData("V6RR-AB0N-9DYH");
            yield return new TestCaseData("JP0X-A0YX-ENY9");
            yield return new TestCaseData("N56N-DQJ6-WGMX");
            yield return new TestCaseData("128C-H2MC-0VHF");
            yield return new TestCaseData("BMCK-YJBV-J08L");
            yield return new TestCaseData("UVA5-WWCC-FKAD");
            yield return new TestCaseData("Y0CL-Q9N7-GLP3");
            yield return new TestCaseData("H41N-PXLC-RX4D");
            yield return new TestCaseData("L5X6-LYL2-XNJ6");
            yield return new TestCaseData("XRKH-0GCN-HDND");
            yield return new TestCaseData("JQTD-N788-XALV");
            yield return new TestCaseData("HPX9-NJ91-2FLV");
            yield return new TestCaseData("GD2K-NCHK-WR71");
            yield return new TestCaseData("0WUV-Y1TX-VJ84");
            yield return new TestCaseData("81DE-XTE8-RTKE");
            yield return new TestCaseData("K7UM-TR8U-E9LX");
            yield return new TestCaseData("U20V-PCXM-PET1");
            yield return new TestCaseData("U3P8-62RD-YVRP");
            yield return new TestCaseData("65X5-PWQV-V4EP");
            yield return new TestCaseData("NDAP-9MJQ-FB5B");
            yield return new TestCaseData("U4X3-YB9H-DVM0");
            yield return new TestCaseData("VDBL-89EL-TDK4");
            yield return new TestCaseData("7A0V-9VUB-B43D");
            yield return new TestCaseData("36WR-15DL-DU6V");
            yield return new TestCaseData("VWLP-2R37-XNL8");
            yield return new TestCaseData("LDX3-86JW-U06C");
            yield return new TestCaseData("0JMX-4GYV-3VWR");
            yield return new TestCaseData("KY4L-V1R0-M4B4");
            yield return new TestCaseData("WJEE-YPC0-MB13");
            yield return new TestCaseData("JLJN-85KB-3DWK");
            yield return new TestCaseData("CHBQ-X2W1-JG5B");
            yield return new TestCaseData("EL4M-DLAF-F54L");
            yield return new TestCaseData("XJ5R-R0B0-UDPV");
            yield return new TestCaseData("VRK8-43FD-MJGU");
            yield return new TestCaseData("0F7M-ARX8-NTA7");
            yield return new TestCaseData("Y4WL-YBHR-HM5Q");
            yield return new TestCaseData("VE85-4PW4-743T");
            yield return new TestCaseData("48D5-F3MN-FW18");
            yield return new TestCaseData("PRAP-CTMU-M6GG");
            yield return new TestCaseData("BUVR-C9UL-2P5A");
            yield return new TestCaseData("U05T-1BC9-3TG5");
            yield return new TestCaseData("GPK9-XYMH-NE9T");
            yield return new TestCaseData("NBDK-MVY7-TLD7");
            yield return new TestCaseData("RJGW-61RR-7EJE");
            yield return new TestCaseData("P8NN-V117-Y81N");
            yield return new TestCaseData("AJJA-UXBM-RHN1");
            yield return new TestCaseData("PUL9-H27W-UJLU");
            yield return new TestCaseData("6DY3-6KX0-EPXU");
            yield return new TestCaseData("AMJ5-K6FW-PHGH");
            yield return new TestCaseData("P4XG-02A2-EAWU");
            yield return new TestCaseData("RJU9-F71G-FRUL");
            yield return new TestCaseData("4DMF-PHRJ-GJCG");
            yield return new TestCaseData("4RGM-MNN8-652Q");
            yield return new TestCaseData("HGPQ-K8XL-QHQE");
            yield return new TestCaseData("51JM-639G-D59G");
            yield return new TestCaseData("NTFU-UVFJ-H4GN");
            yield return new TestCaseData("3Y8E-DT3W-5BVL");
            yield return new TestCaseData("PG0V-K2JJ-KGE9");
            yield return new TestCaseData("K617-5XCG-635L");
            yield return new TestCaseData("ETQ0-VY1K-EP41");
            yield return new TestCaseData("RK89-2L79-NYML");
            yield return new TestCaseData("PF6F-CEVP-TK17");
            yield return new TestCaseData("V41R-8274-VQRB");
            yield return new TestCaseData("A7KK-K46D-96EN");
            yield return new TestCaseData("X1AH-7078-EXJQ");
            yield return new TestCaseData("4JD9-XXU4-RCP0");
            yield return new TestCaseData("HWJJ-AWWM-RVQT");
            yield return new TestCaseData("R90V-V3HX-L2JF");
            yield return new TestCaseData("WCT5-MY68-PQT5");
            yield return new TestCaseData("2Y0H-Q207-B8C5");
            yield return new TestCaseData("0T28-H1PT-DRAR");
            yield return new TestCaseData("7WH3-96VV-8R8G");
            yield return new TestCaseData("T99T-BLHD-ENX8");
            yield return new TestCaseData("UAGA-4K7X-ENMX");
            yield return new TestCaseData("DVXQ-NXHL-9X21");
            yield return new TestCaseData("C3B6-8ALE-VGRE");
            yield return new TestCaseData("4AED-1JGN-GAFN");
            yield return new TestCaseData("WE2K-LPJ3-HEN1");
            yield return new TestCaseData("F1G2-1M45-GT34");
            yield return new TestCaseData("EY09-H0EW-6WM3");
            yield return new TestCaseData("R0K0-VJNA-R0HE");
            yield return new TestCaseData("ERXJ-2LEG-VG9W");
            yield return new TestCaseData("EH1P-7VFM-68AU");
            yield return new TestCaseData("NWNW-P3NV-9W2D");
            yield return new TestCaseData("2AC2-K0YQ-H8PC");
            yield return new TestCaseData("C8PL-PC1P-CATF");
            yield return new TestCaseData("LFER-VJ9V-DH0J");
            yield return new TestCaseData("KDCU-DDTN-J92X");
            yield return new TestCaseData("65AG-U872-5T86");
            yield return new TestCaseData("1N0C-5JHA-TAK9");
            yield return new TestCaseData("5P4N-DJ86-FFNB");
            yield return new TestCaseData("98KJ-ALHQ-TQGQ");
            yield return new TestCaseData("VEFC-A42F-JFXH");
            yield return new TestCaseData("8462-AGRJ-LMTC");
            yield return new TestCaseData("D9W3-1KNG-0F65");
            yield return new TestCaseData("05W8-UEYG-37NN");
            yield return new TestCaseData("ETLU-BF5X-38H5");
            yield return new TestCaseData("RRVK-WFBQ-C978");
            yield return new TestCaseData("97CP-D5HG-X21C");
            yield return new TestCaseData("3JHQ-MDPQ-BWWJ");
            yield return new TestCaseData("KT2G-B01L-4G05");
            yield return new TestCaseData("BU74-J4VF-WNJH");
            yield return new TestCaseData("URY0-CPKX-VFRT");
            yield return new TestCaseData("J7BG-790H-4QW0");
            yield return new TestCaseData("0HNC-JD58-QH2P");
            yield return new TestCaseData("CVY5-JJ9E-UN8W");
            yield return new TestCaseData("BGEK-TVBR-2JGK");
            yield return new TestCaseData("TMPL-0W9J-M0T5");
            yield return new TestCaseData("AR67-TCYL-6447");
            yield return new TestCaseData("TKG6-T7VF-F9TR");
            yield return new TestCaseData("V8U3-CJP1-A5Q2");
            yield return new TestCaseData("2G8K-0H42-8ETB");
            yield return new TestCaseData("029Q-5E7H-LCLM");
            yield return new TestCaseData("JFBD-1YT0-3LWV");
            yield return new TestCaseData("VCPD-E04M-VU6J");
            yield return new TestCaseData("JAHH-Q12M-WJF0");
            yield return new TestCaseData("EJKW-7PQV-A41N");
            yield return new TestCaseData("GGR6-0N77-PMP7");
            yield return new TestCaseData("8VCX-CCM9-E2FG");
            yield return new TestCaseData("FAHK-VLQK-4MQ0");
            yield return new TestCaseData("1PBB-2RPU-2M86");
            yield return new TestCaseData("X3XD-87XW-YTPE");
            yield return new TestCaseData("3XUE-2BUD-DF6H");
            yield return new TestCaseData("XJXK-LJP6-217V");
            yield return new TestCaseData("D57W-LWDK-W20N");
            yield return new TestCaseData("EFN6-WFCR-N7GQ");
            yield return new TestCaseData("PV8G-67LA-NY21");
            yield return new TestCaseData("HHG5-3HYU-RTE9");
            yield return new TestCaseData("G6LV-8R55-AFYD");
            yield return new TestCaseData("4HV5-7GA5-7RG4");
            yield return new TestCaseData("QK3F-24JT-F7H9");
            yield return new TestCaseData("5VJ7-BKCL-5WYQ");
            yield return new TestCaseData("7F32-EJWG-06BR");
            yield return new TestCaseData("4BJ5-8JTH-GNL7");
            yield return new TestCaseData("9LKW-ERC8-T7MG");
            yield return new TestCaseData("BGT0-Q2BJ-LQ9M");
            yield return new TestCaseData("8V8T-MU0K-4J4G");
            yield return new TestCaseData("BEXV-YDQ8-A1LF");
            yield return new TestCaseData("Q97F-XDGB-CN00");
            yield return new TestCaseData("7VRP-PT1A-7F7N");
            yield return new TestCaseData("7594-GXPX-Y0VM");
            yield return new TestCaseData("5X73-6YT7-3DH7");
            yield return new TestCaseData("8XYR-XKF0-LT5Q");
            yield return new TestCaseData("F5RR-6Y9M-DHBW");
            yield return new TestCaseData("QTMB-4KQG-TW2C");
            yield return new TestCaseData("57G9-GQ6P-PFVN");
            yield return new TestCaseData("B35B-9WTW-F3Y9");
            yield return new TestCaseData("661T-W696-6UDK");
            yield return new TestCaseData("9U4P-E2XP-T56R");
            yield return new TestCaseData("429B-B19G-NYGF");
            yield return new TestCaseData("MPEB-FFK0-DLK1");
            yield return new TestCaseData("V44V-MV4B-R8LE");
            yield return new TestCaseData("4NFR-5VXU-RFKN");
            yield return new TestCaseData("YPQ4-AEG2-5W3U");
            yield return new TestCaseData("FXP2-321N-H14F");
            yield return new TestCaseData("95HM-E2E7-019M");
            yield return new TestCaseData("91XL-1CYG-7M38");
            yield return new TestCaseData("URPP-3XE9-MVDV");
            yield return new TestCaseData("X8H2-Y4VU-AP0A");
            yield return new TestCaseData("Y4E5-X3TH-UGBB");
            yield return new TestCaseData("6693-E5G4-JFTD");
            yield return new TestCaseData("45UQ-674R-V9DQ");
            yield return new TestCaseData("4TPG-55YR-MTNX");
            yield return new TestCaseData("M4R2-Q43H-C4GF");
            yield return new TestCaseData("7XW3-JG64-QHG6");
            yield return new TestCaseData("N9JH-D739-UYFM");
            yield return new TestCaseData("JAAA-B8G1-HKXB");
            yield return new TestCaseData("8X5X-FT48-VPLJ");
            yield return new TestCaseData("8RUK-FUVL-T56R");
            yield return new TestCaseData("H9UK-LG5C-9GT7");
            yield return new TestCaseData("K2YP-ET7N-ENR3");
            yield return new TestCaseData("XDD0-W313-43FM");
            yield return new TestCaseData("FDA7-D3A2-GRDT");
            yield return new TestCaseData("UYNC-BHDE-HUR3");
            yield return new TestCaseData("X6HT-M1C2-TXKH");
            yield return new TestCaseData("A427-4LKX-8WH8");
            yield return new TestCaseData("FARU-LETT-TTUB");
            yield return new TestCaseData("YND5-05H4-J0V9");
            yield return new TestCaseData("UJ8X-F8NR-YG5P");
            yield return new TestCaseData("RWU1-F8W1-LD5R");
            yield return new TestCaseData("EUC7-CYLV-EMQE");
            yield return new TestCaseData("QCMQ-H8CQ-XNC0");
            yield return new TestCaseData("NBJQ-KDTJ-EMUH");
            yield return new TestCaseData("JJ2X-P1UT-GBRL");
            yield return new TestCaseData("JQPA-4WXT-QB00");
            yield return new TestCaseData("AAWQ-VY2L-PUDJ");
            yield return new TestCaseData("YX5R-812B-HWTB");
            yield return new TestCaseData("EL7Q-2V98-T1RX");
            yield return new TestCaseData("BXJA-J5QX-T9UW");
            yield return new TestCaseData("V1A8-PVM6-QND5");
            yield return new TestCaseData("JLQV-T698-J1BB");
            yield return new TestCaseData("8RNE-0NUU-9AUJ");
            yield return new TestCaseData("24TT-NM8T-58WT");
            yield return new TestCaseData("0F5K-6DV8-CU35");
            yield return new TestCaseData("CQUJ-0UCE-YT6V");
            yield return new TestCaseData("J1DV-CYBK-95H6");
            yield return new TestCaseData("PMGF-518K-6KR3");
            yield return new TestCaseData("VU3X-GCAG-1M1A");
            yield return new TestCaseData("4DA4-0XRN-4DLX");
            yield return new TestCaseData("CK7U-A0QP-100D");
            yield return new TestCaseData("Y2YF-5PRL-J5YE");
            yield return new TestCaseData("7VTQ-F1WP-V9YB");
            yield return new TestCaseData("V88F-EWLX-Y1N3");
            yield return new TestCaseData("32T8-00L5-WMP3");
            yield return new TestCaseData("BVP8-QPKN-YTTH");
            yield return new TestCaseData("RRRG-N7AA-Y5WQ");
            yield return new TestCaseData("6MCC-AJKK-0HLT");
            yield return new TestCaseData("D251-9V3J-HJ2T");
            yield return new TestCaseData("59TT-ABG7-E05X");
            yield return new TestCaseData("9TEE-0C2W-FT7K");
            yield return new TestCaseData("JBD1-J09D-X0HM");
            yield return new TestCaseData("WK9V-C2K3-5C9L");
            yield return new TestCaseData("UMVE-BK9H-DA8H");
            yield return new TestCaseData("N637-6PCU-NH2E");
            yield return new TestCaseData("5900-VA3R-844G");
            yield return new TestCaseData("B8W6-RQHV-0Q8P");
            yield return new TestCaseData("YH8W-5YJA-6TPA");
            yield return new TestCaseData("LL9N-D3JA-UR9R");
            yield return new TestCaseData("3KC7-2CNU-Q72K");
            yield return new TestCaseData("LBXU-X6YH-CFC3");
            yield return new TestCaseData("922B-A16Q-LARK");
            yield return new TestCaseData("66D7-T7M8-21G6");
            yield return new TestCaseData("MVHG-UU7N-THKJ");
            yield return new TestCaseData("Q1GU-XK0G-60YL");
            yield return new TestCaseData("YNWM-WAYB-ADLT");
            yield return new TestCaseData("VQGH-R3BT-79RK");
            yield return new TestCaseData("LRVC-YPRD-14XT");
            yield return new TestCaseData("PC5K-PB03-8020");
            yield return new TestCaseData("8JDU-HNCB-XNH5");
            yield return new TestCaseData("K2RH-VU04-Y3XJ");
            yield return new TestCaseData("4KL4-WA1C-GY7A");
            yield return new TestCaseData("149L-CXXK-4RC2");
            yield return new TestCaseData("Y653-UG80-GCGX");
            yield return new TestCaseData("B7QD-P73P-1HNH");
            yield return new TestCaseData("EA1E-YNHH-D1E7");
            yield return new TestCaseData("3XBW-5FB9-EEHW");
            yield return new TestCaseData("U19J-30UA-PNHL");
            yield return new TestCaseData("UMXG-BJB0-UH7T");
            yield return new TestCaseData("D56V-LYBQ-9BHU");
            yield return new TestCaseData("N1LN-NA0T-CRQK");
            yield return new TestCaseData("NRG9-10FL-94PQ");
            yield return new TestCaseData("L53A-DYKG-5H6K");
            yield return new TestCaseData("TL8H-WETL-HGP9");
            yield return new TestCaseData("BWCG-2P96-TT7N");
            yield return new TestCaseData("1BK8-CJR3-13CL");
            yield return new TestCaseData("Q2ED-6G17-AM7A");
            yield return new TestCaseData("KXVR-MDQR-1CPG");
            yield return new TestCaseData("0LJ3-86W9-QJX8");
            yield return new TestCaseData("DN8C-6PL4-G4DX");
            yield return new TestCaseData("9FP0-J3XW-Q2DV");
            yield return new TestCaseData("KRKE-U60K-80FD");
            yield return new TestCaseData("J0C8-FAFR-088W");
            yield return new TestCaseData("N0N5-PKVV-Q019");
            yield return new TestCaseData("1Y74-PPH0-CHRP");
            yield return new TestCaseData("DE4B-ME7U-J22M");
            yield return new TestCaseData("NAE1-DNWA-YJRK");
            yield return new TestCaseData("C5GJ-XF24-FLC3");
            yield return new TestCaseData("3E1R-MD01-7FEW");
            yield return new TestCaseData("WGKC-MJ69-HW9R");
            yield return new TestCaseData("VA2G-KGE1-9CPM");
            yield return new TestCaseData("ACFH-EMP5-VVMM");
            yield return new TestCaseData("MQM6-FTBF-LPTK");
            yield return new TestCaseData("HXBX-UKM8-H0C4");
            yield return new TestCaseData("0PCP-KHU2-17KA");
            yield return new TestCaseData("1Y41-869L-H9Q1");
            yield return new TestCaseData("GAVK-B14B-VUYC");
            yield return new TestCaseData("CMEA-3MJV-TMM3");
            yield return new TestCaseData("YBWW-BPB2-AN5U");
            yield return new TestCaseData("KGV7-4N7R-CXYV");
            yield return new TestCaseData("T5DG-X67Q-0JPH");
            yield return new TestCaseData("HMTW-AX5G-TBHT");
            yield return new TestCaseData("M212-4L6H-Y1U8");
            yield return new TestCaseData("B4UM-JYRW-40JW");
            yield return new TestCaseData("H7AT-5QUA-TWDP");
            yield return new TestCaseData("HYBJ-P57L-GE3Q");
            yield return new TestCaseData("B2D0-88M8-UNMB");
            yield return new TestCaseData("4GXK-U4M2-LXNQ");
            yield return new TestCaseData("4NFR-93MT-3570");
            yield return new TestCaseData("NAK6-B8M6-E3C1");
            yield return new TestCaseData("3182-K713-BRJQ");
            yield return new TestCaseData("QWCV-HN54-2EB0");
            yield return new TestCaseData("E7TD-HWV5-P02M");
            yield return new TestCaseData("P28J-XBXJ-VDNG");
            yield return new TestCaseData("62L0-99UN-L0LA");
            yield return new TestCaseData("GTTB-17BR-N6G5");
            yield return new TestCaseData("GQFQ-9688-R259");
            yield return new TestCaseData("N2B1-LV8T-8D85");
            yield return new TestCaseData("MA86-0Y6N-F03H");
            yield return new TestCaseData("XEDK-587U-2JX2");
            yield return new TestCaseData("C9XF-YM4G-VNLX");
            yield return new TestCaseData("BB58-A33V-5E0J");
            yield return new TestCaseData("BR3Q-42EQ-H5YR");
            yield return new TestCaseData("9DNP-QGYR-LWM4");
            yield return new TestCaseData("7T7W-2LQT-DHRC");
            yield return new TestCaseData("8KW0-MMB9-PTDX");
            yield return new TestCaseData("VRF4-M1NC-NARV");
            yield return new TestCaseData("U6V8-WC2L-KHK2");
            yield return new TestCaseData("Q7JK-8PG9-426Q");
            yield return new TestCaseData("VC8W-VU8C-88LF");
            yield return new TestCaseData("KGP2-87DC-N4Q6");
            yield return new TestCaseData("QYQG-870X-A7RA");
            yield return new TestCaseData("5AYK-DL16-2MDB");
            yield return new TestCaseData("JMF7-3HXT-4UHE");
            yield return new TestCaseData("8VAV-MNXG-GMVU");
            yield return new TestCaseData("D6HU-B7PL-2JQU");
            yield return new TestCaseData("PQ2U-XEWC-369M");
            yield return new TestCaseData("HKQL-88TD-BQH4");
            yield return new TestCaseData("LH2L-RN04-47UG");
            yield return new TestCaseData("YJGR-YM0C-2U5Q");
            yield return new TestCaseData("U87R-DU1F-9TJ3");
            yield return new TestCaseData("FVW1-D6J5-3FEB");
            yield return new TestCaseData("CDLK-HYN6-WMCP");
            yield return new TestCaseData("GXQP-1KF9-DXCW");
            yield return new TestCaseData("WA8B-JN7T-BH5E");
            yield return new TestCaseData("V1WU-Y1EJ-Y3PB");
            yield return new TestCaseData("8RTJ-VMUA-9K7E");
            yield return new TestCaseData("43LA-JWPL-529G");
            yield return new TestCaseData("02XE-3YYE-76GF");
            yield return new TestCaseData("HRF1-RU9F-R9L2");
            yield return new TestCaseData("GN35-1HF2-A5Y9");
            yield return new TestCaseData("1A9A-C91R-NUG1");
            yield return new TestCaseData("10HE-600C-VUXB");
            yield return new TestCaseData("7H6C-M94N-GX3J");
            yield return new TestCaseData("F1XG-DBPC-4FAU");
            yield return new TestCaseData("3TPU-J10P-U4VH");
            yield return new TestCaseData("CM2W-FLL3-0GPA");
            yield return new TestCaseData("J6DX-HMM1-L3UC");
            yield return new TestCaseData("9V18-U16P-HXJN");
            yield return new TestCaseData("QJLD-VL3W-EKEV");
            yield return new TestCaseData("UMP9-57WX-F648");
            yield return new TestCaseData("7KR7-1RWD-K3UP");
            yield return new TestCaseData("BLBX-XEYE-F41W");
            yield return new TestCaseData("KVK9-E149-8VHL");
            yield return new TestCaseData("18GA-9KWV-WW6E");
            yield return new TestCaseData("LBJF-0280-HLHH");
            yield return new TestCaseData("L6QK-G9T6-KGJD");
            yield return new TestCaseData("KNKK-UJQP-998B");
            yield return new TestCaseData("149L-RKQ2-Q7XG");
            yield return new TestCaseData("7GYJ-9XC3-HYH9");
            yield return new TestCaseData("L3AA-022R-BQL7");
            yield return new TestCaseData("KENR-M9WG-FCVN");
            yield return new TestCaseData("BHRJ-PKKK-HL44");
            yield return new TestCaseData("JU3G-6TT5-UL8N");
            yield return new TestCaseData("QJRJ-1813-P9AE");
            yield return new TestCaseData("J5GE-L9LJ-9G7K");
            yield return new TestCaseData("YVL2-43RP-9RGD");
            yield return new TestCaseData("QMQC-DDEA-0RW1");
            yield return new TestCaseData("EXYM-8FB7-AYPX");
            yield return new TestCaseData("1FFJ-NMTD-QV3G");
            yield return new TestCaseData("C2Q0-KDMD-YD80");
            yield return new TestCaseData("PRT8-WL3J-K6XM");
            yield return new TestCaseData("9M53-N3VE-HBDV");
            yield return new TestCaseData("5JCV-XYMH-NCFR");
            yield return new TestCaseData("WRDN-YNFF-QP1C");
            yield return new TestCaseData("3C5N-8M8R-WPUE");
            yield return new TestCaseData("VWPT-QJ89-J27T");
            yield return new TestCaseData("C80V-XYMH-6742");
            yield return new TestCaseData("VHWM-AR3C-HGF1");
            yield return new TestCaseData("QAKF-MU9V-3RDE");
            yield return new TestCaseData("QL00-J517-ALFX");
            yield return new TestCaseData("7Y92-HRJC-QT9F");
            yield return new TestCaseData("5FUH-YUQT-WU9A");
            yield return new TestCaseData("HV6J-1HH4-GF4D");
            yield return new TestCaseData("QM8U-JK0Q-KBXP");
            yield return new TestCaseData("EHPE-A6C1-7YMX");
            yield return new TestCaseData("5QEL-5MNA-HBCU");
            yield return new TestCaseData("XX55-B4RU-ULBR");
            yield return new TestCaseData("GM3H-AJ00-LTRD");
            yield return new TestCaseData("M8H8-MHBT-GPT1");
            yield return new TestCaseData("JCT2-9GQV-BU8M");
            yield return new TestCaseData("BL3N-0NJJ-3PVN");
            yield return new TestCaseData("3GAA-FWNM-CW1A");
            yield return new TestCaseData("RFUE-5UX8-RQF3");
            yield return new TestCaseData("ED19-HQR0-0KXC");
            yield return new TestCaseData("NJ8P-3UTT-9GGV");
            yield return new TestCaseData("145G-XPRQ-2WTM");
            yield return new TestCaseData("F053-7RR5-G3RP");
            yield return new TestCaseData("CQQF-9983-R0QM");
            yield return new TestCaseData("XML4-AJBB-PJ1M");
            yield return new TestCaseData("CQWL-UE9R-LG1G");
            yield return new TestCaseData("2EBF-DV8A-F0M4");
            yield return new TestCaseData("N34D-CD6D-E4NX");
            yield return new TestCaseData("71U8-5KK0-UE1R");
            yield return new TestCaseData("4Y94-Q93K-WRB5");
            yield return new TestCaseData("E2N7-02PF-WLH9");
            yield return new TestCaseData("QE1B-MKUJ-CKNU");
            yield return new TestCaseData("LUVK-2YNG-3LKJ");
            yield return new TestCaseData("86CF-U3ME-WQCJ");
            yield return new TestCaseData("18NG-LBLR-95UG");
            yield return new TestCaseData("3R4L-9JW9-RHV7");
            yield return new TestCaseData("FH4F-212F-5ABF");
            yield return new TestCaseData("AM8T-YPL8-5H0D");
            yield return new TestCaseData("3NTG-6ATB-UT15");
            yield return new TestCaseData("FCJU-T05E-Q48X");
            yield return new TestCaseData("HHWJ-46YQ-H5A4");
            yield return new TestCaseData("PRT8-M8QP-2NDX");
            yield return new TestCaseData("QVWT-FTW2-HBXE");
            yield return new TestCaseData("B5P5-RBU8-3LGF");
            yield return new TestCaseData("M42A-69DA-WNKJ");
            yield return new TestCaseData("3F6J-XFQT-JRND");
            yield return new TestCaseData("K5PA-UAQT-88RL");
            yield return new TestCaseData("XTWF-N9GP-E3XK");
            yield return new TestCaseData("FNT8-765U-VA98");
            yield return new TestCaseData("DUFM-Y8GW-EYXQ");
            yield return new TestCaseData("G5K8-73EA-4D5F");
            yield return new TestCaseData("9M53-21Q6-TV3R");
            yield return new TestCaseData("Y4C3-3BY6-YU2C");
            yield return new TestCaseData("W2BH-MMPM-0AB8");
            yield return new TestCaseData("38Y3-03Q4-GAU3");
            yield return new TestCaseData("9NBV-6LC1-5R4E");
            yield return new TestCaseData("Y1UP-C03B-RR8F");
            yield return new TestCaseData("YAAN-L0EU-FU11");
            yield return new TestCaseData("46P8-KHY6-NDDB");
            yield return new TestCaseData("P08B-J517-QDDL");
            yield return new TestCaseData("Q8J7-RM3K-87KT");
            yield return new TestCaseData("TK5T-HY7N-VYUN");
            yield return new TestCaseData("0TMU-X17N-FE78");
            yield return new TestCaseData("8RJA-ARBL-3XFJ");
            yield return new TestCaseData("K39L-5LHH-563Q");
            yield return new TestCaseData("DYK8-AVHM-HKT7");
            yield return new TestCaseData("GCKH-D93G-N3MF");
            yield return new TestCaseData("EJR4-K8B1-FLC3");
            yield return new TestCaseData("1DXT-J9B0-4L3N");
            yield return new TestCaseData("0UPH-UBXL-YCMR");
            yield return new TestCaseData("MJFA-VJL8-JHA4");
            yield return new TestCaseData("JUFV-GG2N-E234");
            yield return new TestCaseData("CCQ4-WXNA-E7CF");
            yield return new TestCaseData("JYC8-48VV-P2JD");
            yield return new TestCaseData("0WGH-QJCD-5Y33");
            yield return new TestCaseData("C95M-Q19V-R7QX");
            yield return new TestCaseData("5LYP-NL32-YPYU");
            yield return new TestCaseData("NDM3-3R1R-XKC5");
            yield return new TestCaseData("HLUB-TXJ8-LNNU");
            yield return new TestCaseData("XTTC-QB1Q-2MLJ");
            yield return new TestCaseData("K2JA-GJ84-MCPD");
            yield return new TestCaseData("PEF5-5H49-QQFE");
            yield return new TestCaseData("0PU7-UARU-JXNF");
            yield return new TestCaseData("X0C0-AL07-0QVC");
            yield return new TestCaseData("QLY0-F6LG-EJLF");
            yield return new TestCaseData("7GH4-J78M-HD2Q");
            yield return new TestCaseData("5K7B-XPFE-AH8V");
            yield return new TestCaseData("PNH4-K43A-X76K");
            yield return new TestCaseData("K30B-AKRD-QG24");
            yield return new TestCaseData("K7MF-MWLF-UQGD");
            yield return new TestCaseData("B3R0-8QBP-JVDX");
            yield return new TestCaseData("GXXW-Y32D-GE8W");
            yield return new TestCaseData("G3YD-63KT-UVXA");
            yield return new TestCaseData("PAU3-V3R7-AWJJ");
            yield return new TestCaseData("JN9L-QGRK-P1Y7");
            yield return new TestCaseData("CGWP-ARQ2-JKY1");
            yield return new TestCaseData("WB4T-L29W-HQDU");
            yield return new TestCaseData("J0GC-2MQE-12WJ");
            yield return new TestCaseData("4GUG-1UQF-20EG");
            yield return new TestCaseData("313V-KLEF-M4KC");
            yield return new TestCaseData("XAKB-QLAJ-J33A");
            yield return new TestCaseData("W098-NAMG-DGNM");
            yield return new TestCaseData("GAA1-L2TF-7UWQ");
            yield return new TestCaseData("DRED-HCTM-FEAB");
            yield return new TestCaseData("BTQ2-1CWE-669K");
        }
    }
}