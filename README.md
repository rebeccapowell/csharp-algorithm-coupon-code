# csharp-algorithm-coupon-code
Implementation of Perl's Algorithm::CouponCode in C#

[![benpowell MyGet Build Status](https://www.myget.org/BuildSource/Badge/benpowell?identifier=291bd95d-8539-41ef-9320-3684077676a1)](https://www.myget.org/)

Port of http://search.cpan.org/dist/Algorithm-CouponCode/

Copyright 2011 Grant McLean grantm@cpan.org

With nods to the various other language ports for guidance:
- https://github.com/baxang/coupon-code/blob/master/lib/coupon_code.rb
- https://github.com/chilts/node-coupon-code/blob/master/coupon-code.js

## Synopsis
 A 'Coupon Code' is made up of letters and numbers grouped into 4 character
 'parts'.  For example, a 3-part code might look like this:
 
 1K7Q-CTFM-LMTC

 Coupon Codes are random codes which are easy for the recipient to type
 accurately into a web form.  An example application might be to print a code on
 a letter to a customer who would then enter the code as part of the
 registration process for web access to their account.

 Features of the codes that make them well suited to manual transcription:
  - The codes are not case sensitive.
  - Not all letters and numbers are used, so if a person enters the letter 'O' we
    can automatically correct it to the digit '0' (similarly for I => 1, S => 5, Z => 2).
 - The 4th character of each part is a checkdigit, so client-side scripting can
    be used to highlight parts which have been mis-typed, before the code is even
    submitted to the application's back-end validation.
 - The checkdigit algorithm takes into account the position of the part being
    keyed.  So for example '1K7Q' might be valid in the first part but not in the
    second so if a user typed the parts in the wrong boxes then their error could
    be highlighted.
 - The code generation algorithm avoids 'undesirable' codes. For example any code
    in which transposed characters happen to result in a valid checkdigit will be
    skipped.  Any generated part which happens to spell an 'inappropriate' 4-letter
    word (e.g.: 'P00P') will also be skipped.
 - The code returned by Generate() is random, but not necessarily unique.
    If your application requires unique codes, it is your responsibility to
    avoid duplicates (for example by using a unique index on your database column).
 - The codes are generated using a SHA1 cryptographic hash of a plaintext.  If you
    do not supply a plaintext, one will be generated for you (using RNGCryptoServiceProvider).  
    In the event that an 'inappropriate' code is created, the generated hash will be 
    used as a plaintext input for generating a new hash and the process will be repeated.
 - Each 4-character part encodes 15 bits of random data, so a 3-part code will
    incorporate 45 bits making a total of 2^45 (approximately 35 trillion) unique
    codes.

## string Generate(Options opts) method:
Returns a coupon code as a string of 4-character parts separated by '-'
characters.  The following optional named parameters may be supplied:

### Parts
The number of parts desired.  Must be a number in the range 1 - 6.  Default is 3.

### Plaintext
A byte string which will be hashed using Digest::SHA to produce the code.
If you do not supply your own plaintext then a random one will be generated for you.

## string Validate(string code, Options opts)

Takes a code, cleans it up and validates the checkdigits.  Returns the
normalised (and untainted) version of the code on success or undef on error.
The following named parameters may be supplied:

### Code
The code to be validated.  The parameter is mandatory.

### Parts
The number of parts you expect the code to contain.  Default is 3.


