using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace eBSH.Helper
{
    public class UnicodeConvert
    {
        public static string UnicodeMap = "áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴĐ";
        public static string[] VIQRMap = new string[] { "a'", "a`", "a?", "a~", "a.", "a(", "a('", "a(`", "a(?", "a(~", "a(.", "a^", "a^'", "a^`", "a^?", "a^~", "a^.", "e'", "e`", "e?", "e~", "e.", "e^", "e^'", "e^`", "e^?", "e^~", "e^.", "i'", "i`", "i?", "i~", "i.", "o'", "o`", "o?", "o~", "o.", "o^", "o^'", "o^`", "o^?", "o^~", "o^.", "o+", "o+'", "o+`", "o+?", "o+~", "o+.", "u'", "u`", "u?", "u~", "u.", "u+", "u+'", "u+`", "u+?", "u+~", "u+.", "y'", "y`", "y?", "y~", "y.", "dd", "A'", "A`", "A?", "A~", "A.", "A(", "A('", "A(`", "A(?", "A(~", "A(.", "A^", "A^'", "A^`", "A^?", "A^~", "A^.", "E'", "E`", "E?", "E~", "E.", "E^", "E^'", "E^`", "E^?", "E^~", "E^.", "I'", "I`", "I?", "I~", "I.", "O'", "O`", "O?", "O~", "O.", "O^", "O^'", "O^`", "O^?", "O^~", "O^.", "O+", "O+'", "O+`", "O+?", "O+~", "O+.", "U'", "U`", "U?", "U~", "U.", "U+", "U+'", "U+`", "U+?", "U+~", "U+.", "Y'", "Y`", "Y?", "Y~", "Y.", "DD" };
        public static string[] CP1258Map = new string[] { "aì","aÌ", "aÒ", "aÞ","aò", "ãì", "ãÌ", "ãÒ", "ãÞ", "ãò", "ã", "âì", "âÌ", "âÒ", "âÞ", "âò", "â", "eì",  "eÌ", "eÒ", "eÞ", "eò",
                                                          "êì", "êÌ", "êÒ","êÞ","êò","ê","iì","iÌ", "iÒ","iÞ","iò","oì","oÌ","oÒ","oÞ","oò","ôì","ôÌ","ôÒ", "ôÞ","ôò","ô",
                                                          "õì","õÌ","õÒ","õÞ","õò","õ","uì","uÌ", "uÒ", "uÞ","uò","ýì","ýÌ","ýÒ", "ýÞ","ýò","ý","yì","yÌ","yÒ","yÞ", "yò","ð",
                                                          "Aì", "AÌ", "AÒ", "AÞ","Aò", "Ãì","ÃÌ","ÃÒ","ÃÞ","Ãò","Ã","Âì", "ÂÌ", "ÂÒ","ÂÞ", "Âò", "Â",
                                                          "Eì'","EÌ","EÒ","EÞ","Eò", "Êì","ÊÌ","ÊÒ","ÊÞ","Êò","Ê","Iì'", "IÌ","IÒ","IÞ","Iò",
                                                          "Oì","OÌ","OÒ","OÞ","Oò", "Ôì","ÔÌ","ÔÒ","ÔÞ","Ôò","Ô", "Õì","ÕÌ","ÕÒ","ÕÞ", "Õò","Õ",
                                                          "Uì","UÌ","UÒ","UÞ","Uò","Ýì'","ÝÌ","ÝÒ","ÝÞ","Ýò","Ý","Yì","YÌ","YÒ","YÞ","Yò","Ð"};
        public static string[] VIQRMapReg = new string[] {  @"a'", "á",
                                                             @"a`", "à",
                                                             @"a\?", "ả",
                                                             @"a~", "ã",
                                                             @"a\.", "ạ",
                                                             @"a\('", "ắ",
                                                             @"a\(`", "ằ",
                                                             @"a\(\?", "ẳ",
                                                             @"a\(~", "ẵ",
                                                             @"a\(\.", "ặ",
                                                             @"a\(", "ă",
                                                             @"a\^'", "ấ",
                                                             @"a\^`", "ầ",
                                                             @"a\^\?", "ẩ",
                                                             @"a\^~", "ẫ",
                                                             @"a\^\.", "ậ",
                                                             @"a\^", "â",
                                                             @"e'", "é",
                                                             @"e`", "è",
                                                             @"e\?", "ẻ",
                                                             @"e~", "ẽ",
                                                             @"e\.", "ẹ",
                                                             @"e\^'", "ế",
                                                             @"e\^`", "ề",
                                                             @"e\^\?", "ể",
                                                             @"e\^~", "ễ",
                                                             @"e\^\.", "ệ",
                                                             @"e\^", "ê",
                                                             @"i'", "í",
                                                             @"i`", "ì",
                                                             @"i\?", "ỉ",
                                                             @"i~", "ĩ",
                                                             @"i\.", "ị",
                                                             @"o'", "ó",
                                                             @"o`", "ò",
                                                             @"o\?", "ỏ",
                                                             @"o~", "õ",
                                                             @"o\.", "ọ",
                                                             @"o\^'", "ố",
                                                             @"o\^`", "ồ",
                                                             @"o\^\?", "ổ",
                                                             @"o\^~", "ỗ",
                                                             @"o\^\.", "ộ",
                                                             @"o\^", "ô",
                                                             @"o\+'", "ớ",
                                                             @"o\+`", "ờ",
                                                             @"o\+\?", "ở",
                                                             @"o\+~", "ỡ",
                                                             @"o\+\.", "ợ",
                                                             @"o\+", "ơ",
                                                             @"u'", "ú",
                                                             @"u`", "ù",
                                                             @"u\?", "ủ",
                                                             @"u~", "ũ",
                                                             @"u\.", "ụ",
                                                             @"u\+'", "ứ",
                                                             @"u\+`", "ừ",
                                                             @"u\+\?", "ử",
                                                             @"u\+~", "ữ",
                                                             @"u\+\.", "ự",
                                                             @"u\+", "ư",
                                                             @"y'", "ý",
                                                             @"y`", "ỳ",
                                                             @"y\?", "ỷ",
                                                             @"y~", "ỹ",
                                                             @"y\.", "ỵ",
                                                             @"dd", "đ",
                                                             @"A'", "Á",
                                                             @"A`", "À",
                                                             @"A\?", "Ả",
                                                             @"A~", "Ã",
                                                             @"A\.", "Ạ",
                                                             @"A\('", "Ắ",
                                                             @"A\(`", "Ằ",
                                                             @"A\(\?", "Ẳ",
                                                             @"A\(~", "Ẵ",
                                                             @"A\(\.", "Ặ",
                                                             @"A\(", "Ă",
                                                             @"A\^'", "Ấ",
                                                             @"A\^`", "Ầ",
                                                             @"A\^\?", "Ẩ",
                                                             @"A\^~", "Ẫ",
                                                             @"A\^\.", "Ậ",
                                                             @"A\^", "Â",
                                                             @"E'", "É",
                                                             @"E`", "È",
                                                             @"E\?", "Ẻ",
                                                             @"E~", "Ẽ",
                                                             @"E\.", "Ẹ",
                                                             @"E\^'", "Ế",
                                                             @"E\^`", "Ề",
                                                             @"E\^\?", "Ể",
                                                             @"E\^~", "Ễ",
                                                             @"E\^\.", "Ệ",
                                                             @"E\^", "Ê",
                                                             @"I'", "Í",
                                                             @"I`", "Ì",
                                                             @"I\?", "Ỉ",
                                                             @"I~", "Ĩ",
                                                             @"I\.", "Ị",
                                                             @"O'", "Ó",
                                                             @"O`", "Ò",
                                                             @"O\?", "Ỏ",
                                                             @"O~", "Õ",
                                                             @"O\.", "Ọ",
                                                             @"O\^'", "Ố",
                                                             @"O\^`", "Ồ",
                                                             @"O\^\?", "Ổ",
                                                             @"O\^~", "Ỗ",
                                                             @"O\^\.", "Ộ",
                                                             @"O\^", "Ô",
                                                             @"O\+'", "Ớ",
                                                             @"O\+`", "Ờ",
                                                             @"O\+\?", "Ở",
                                                             @"O\+~", "Ỡ",
                                                             @"O\+\.", "Ợ",
                                                             @"O\+", "Ơ",
                                                             @"U'", "Ú",
                                                             @"U`", "Ù",
                                                             @"U\?", "Ủ",
                                                             @"U~", "Ũ",
                                                             @"U\.", "Ụ",
                                                             @"U\+'", "Ứ",
                                                             @"U\+`", "Ừ",
                                                             @"U\+\?", "Ử",
                                                             @"U\+~", "Ữ",
                                                             @"U\+\.", "Ự",
                                                             @"U\+", "Ư",
                                                             @"Y'", "Ý",
                                                             @"Y`", "Ỳ",
                                                             @"Y\?", "Ỷ",
                                                             @"Y~", "Ỹ",
                                                             @"Y\.", "Ỵ",
                                                             @"DD", "Đ"};
        //  var CP1258 ="@@";
        //var uni = "úùủũụư@ứừửữựýỳỷỹỵđ@";

        public static string[] CP1258MapReg = new string[] {  @"aì", "á",
                                                             @"aÌ", "à",
                                                             @"aÒ", "ả",
                                                             @"aÞ", "ã",
                                                             @"aò", "ạ",
                                                             @"ãì", "ắ",
                                                             @"ãÌ", "ằ",
                                                             @"ãÒ", "ẳ",
                                                             @"ãÞ", "ẵ",
                                                             @"ãò", "ặ",
                                                             @"ã", "ă",
                                                             @"âì", "ấ",
                                                             @"âÌ", "ầ",
                                                             @"âÒ", "ẩ",
                                                             @"âÞ", "ẫ",
                                                             @"âò", "ậ",
                                                             @"â", "â",
                                                             @"eì", "é",
                                                             @"eÌ", "è",
                                                             @"eÒ", "ẻ",
                                                             @"eÞ", "ẽ",
                                                             @"eò", "ẹ",
                                                             @"êì", "ế",
                                                             @"êÌ", "ề",
                                                             @"êÒ", "ể",
                                                             @"êÞ", "ễ",
                                                             @"êò", "ệ",
                                                             @"ê", "ê",
                                                             @"iì", "í",
                                                             @"iÌ", "ì",
                                                             @"iÒ", "ỉ",
                                                             @"iÞ", "ĩ",
                                                             @"iò", "ị",
                                                             @"oì", "ó",
                                                             @"oÌ", "ò",
                                                             @"oÒ", "ỏ",
                                                             @"oÞ", "õ",
                                                             @"oò", "ọ",
                                                             @"ôì", "ố",
                                                             @"ôÌ", "ồ",
                                                             @"ôÒ", "ổ",
                                                             @"ôÞ", "ỗ",
                                                             @"ôò", "ộ",
                                                             @"ô", "ô",
                                                             @"õì", "ớ",
                                                             @"õÌ", "ờ",
                                                             @"õÒ", "ở",
                                                             @"õÞ", "ỡ",
                                                             @"õò", "ợ",
                                                             @"õ", "ơ",
                                                             @"uì", "ú",
                                                             @"uÌ", "ù",
                                                             @"uÒ", "ủ",
                                                             @"uÞ", "ũ",
                                                             @"uò", "ụ",
                                                             @"ýì", "ứ",
                                                             @"ýÌ", "ừ",
                                                             @"ýÒ", "ử",
                                                             @"ýÞ", "ữ",
                                                             @"ýò", "ự",
                                                             @"ý", "ư",
                                                             @"yì", "ý",
                                                             @"yÌ", "ỳ",
                                                             @"yÒ", "ỷ",
                                                             @"yÞ", "ỹ",
                                                             @"yò", "ỵ",
                                                             @"ð", "đ",
                                                             @"Aì", "Á",
                                                             @"AÌ", "À",
                                                             @"AÒ", "Ả",
                                                             @"AÞ", "Ã",
                                                             @"Aò", "Ạ",
                                                             @"Ãì", "Ắ",
                                                             @"ÃÌ", "Ằ",
                                                             @"ÃÒ", "Ẳ",
                                                             @"ÃÞ", "Ẵ",
                                                             @"Ãò", "Ặ",
                                                             @"Ã", "Ă",
                                                             @"Âì", "Ấ",
                                                             @"ÂÌ", "Ầ",
                                                             @"ÂÒ", "Ẩ",
                                                             @"ÂÞ", "Ẫ",
                                                             @"Âò", "Ậ",
                                                             @"Â", "Â",
                                                             @"Eì", "É",
                                                             @"EÌ", "È",
                                                             @"EÒ", "Ẻ",
                                                             @"EÞ", "Ẽ",
                                                             @"Eò", "Ẹ",
                                                             @"Êì", "Ế",
                                                             @"ÊÌ", "Ề",
                                                             @"ÊÒ", "Ể",
                                                             @"ÊÞ", "Ễ",
                                                             @"Êò", "Ệ",
                                                             @"Ê", "Ê",
                                                             @"Iì'", "Í",
                                                             @"IÌ", "Ì",
                                                             @"IÒ", "Ỉ",
                                                             @"IÞ", "Ĩ",
                                                             @"Iò", "Ị",
                                                             @"Oì", "Ó",
                                                             @"OÌ", "Ò",
                                                             @"OÒ", "Ỏ",
                                                             @"OÞ", "Õ",
                                                             @"Oò", "Ọ",
                                                             @"Ôì", "Ố",
                                                             @"ÔÌ", "Ồ",
                                                             @"ÔÒ", "Ổ",
                                                             @"ÔÞ", "Ỗ",
                                                             @"Ôò", "Ộ",
                                                             @"Ô", "Ô",
                                                             @"Õì", "Ớ",
                                                             @"ÕÌ", "Ờ",
                                                             @"ÕÒ", "Ở",
                                                             @"ÕÞ", "Ỡ",
                                                             @"Õò", "Ợ",
                                                             @"Õ", "Ơ",
                                                             @"Uì", "Ú",
                                                             @"UÌ", "Ù",
                                                             @"UÒ", "Ủ",
                                                             @"UÞ", "Ũ",
                                                             @"Uò", "Ụ",
                                                             @"Ýì'", "Ứ",
                                                             @"ÝÌ", "Ừ",
                                                             @"ÝÒ", "Ử",
                                                             @"ÝÞ", "Ữ",
                                                             @"Ýò", "Ự",
                                                             @"Ý", "Ư",
                                                             @"Yì", "Ý",
                                                             @"YÌ", "Ỳ",
                                                             @"YÒ", "Ỷ",
                                                             @"YÞ", "Ỹ",
                                                             @"Yò", "Ỵ",
                                                             @"Ð", "Đ"};
        public static string[] UComposeMapReg = new string[] {  @"á", "á",
                                                             @"à", "à",
                                                             @"ả", "ả",
                                                             @"ã", "ã",
                                                             @"ạ", "ạ",
                                                             @"ắ", "ắ",
                                                             @"ằ", "ằ",
                                                             @"ẳ", "ẳ",
                                                             @"ẵ", "ẵ",
                                                             @"ặ", "ặ",
                                                             @"ă", "ă",
                                                             @"ấ", "ấ",
                                                             @"ầ", "ầ",
                                                             @"ẩ", "ẩ",
                                                             @"ẫ", "ẫ",
                                                             @"ậ", "ậ",
                                                             @"â", "â",
                                                             @"é", "é",
                                                             @"è", "è",
                                                             @"ẻ", "ẻ",
                                                             @"ẽ", "ẽ",
                                                             @"ẹ", "ẹ",
                                                             @"ế", "ế",
                                                             @"ề", "ề",
                                                             @"ể", "ể",
                                                             @"ễ", "ễ",
                                                             @"ệ", "ệ",
                                                             @"ê", "ê",
                                                             @"í", "í",
                                                             @"ì", "ì",
                                                             @"ỉ", "ỉ",
                                                             @"ĩ", "ĩ",
                                                             @"ị", "ị",
                                                             @"ó", "ó",
                                                             @"ò", "ò",
                                                             @"ỏ", "ỏ",
                                                             @"õ", "õ",
                                                             @"ọ", "ọ",
                                                             @"ố", "ố",
                                                             @"ồ", "ồ",
                                                             @"ổ", "ổ",
                                                             @"ỗ", "ỗ",
                                                             @"ộ", "ộ",
                                                             @"ô", "ô",
                                                             @"ớ", "ớ",
                                                             @"ờ", "ờ",
                                                             @"ở", "ở",
                                                             @"ỡ", "ỡ",
                                                             @"ợ", "ợ",
                                                             @"ơ", "ơ",
                                                             @"ú", "ú",
                                                             @"ù", "ù",
                                                             @"ủ", "ủ",
                                                             @"ũ", "ũ",
                                                             @"ụ", "ụ",
                                                             @"ứ", "ứ",
                                                             @"ừ", "ừ",
                                                             @"ử", "ử",
                                                             @"ữ", "ữ",
                                                             @"ự", "ự",
                                                             @"ư", "ư",
                                                             @"ý", "ý",
                                                             @"ỳ", "ỳ",
                                                             @"ỷ", "ỷ",
                                                             @"ỹ", "ỹ",
                                                             @"ỵ", "ỵ",
                                                             @"đ", "đ",
                                                             @"Á", "Á",
                                                             @"À", "À",
                                                             @"Ả", "Ả",
                                                             @"Ã", "Ã",
                                                             @"Ạ", "Ạ",
                                                             @"Ắ", "Ắ",
                                                             @"Ằ", "Ằ",
                                                             @"Ẳ", "Ẳ",
                                                             @"Ẵ", "Ẵ",
                                                             @"Ặ", "Ặ",
                                                             @"Ă", "Ă",
                                                             @"Ấ", "Ấ",
                                                             @"Ầ", "Ầ",
                                                             @"Ẩ", "Ẩ",
                                                             @"Ẫ", "Ẫ",
                                                             @"Ậ", "Ậ",
                                                             @"Â", "Â",
                                                             @"É", "É",
                                                             @"È", "È",
                                                             @"Ẻ", "Ẻ",
                                                             @"Ẽ", "Ẽ",
                                                             @"Ẹ", "Ẹ",
                                                             @"Ế", "Ế",
                                                             @"Ề", "Ề",
                                                             @"Ể", "Ể",
                                                             @"Ễ", "Ễ",
                                                             @"Ệ", "Ệ",
                                                             @"Ê", "Ê",
                                                             @"Í'", "Í",
                                                             @"Ì", "Ì",
                                                             @"Ỉ", "Ỉ",
                                                             @"Ĩ", "Ĩ",
                                                             @"Ị", "Ị",
                                                             @"Ó", "Ó",
                                                             @"Ò", "Ò",
                                                             @"Ỏ", "Ỏ",
                                                             @"Õ", "Õ",
                                                             @"Ọ", "Ọ",
                                                             @"Ố", "Ố",
                                                             @"Ồ", "Ồ",
                                                             @"Ổ", "Ổ",
                                                             @"Ỗ", "Ỗ",
                                                             @"Ộ", "Ộ",
                                                             @"Ô", "Ô",
                                                             @"Ớ", "Ớ",
                                                             @"Ờ", "Ờ",
                                                             @"Ở", "Ở",
                                                             @"Ỡ", "Ỡ",
                                                             @"Ợ", "Ợ",
                                                             @"Ơ", "Ơ",
                                                             @"Ú", "Ú",
                                                             @"Ù", "Ù",
                                                             @"Ủ", "Ủ",
                                                             @"Ũ", "Ũ",
                                                             @"Ụ", "Ụ",
                                                             @"Ứ'", "Ứ",
                                                             @"Ừ", "Ừ",
                                                             @"Ử", "Ử",
                                                             @"Ữ", "Ữ",
                                                             @"Ự", "Ự",
                                                             @"Ư", "Ư",
                                                             @"Ý", "Ý",
                                                             @"Ỳ", "Ỳ",
                                                             @"Ỷ", "Ỷ",
                                                             @"Ỹ", "Ỹ",
                                                             @"Ỵ", "Ỵ",
                                                             @"Đ", "Đ"};

        public static string TCVN3Map = "¸µ¶·¹¨¾»¼½Æ©ÊÇÈÉËÐÌÎÏÑªÕÒÓÔÖÝ×ØÜÞãßáâä«èåæçé¬íêëìîóïñòô­øõö÷ùýúûüþ®¸µ¶·¹¡¾»¼½Æ¢ÊÇÈÉËÐÌÎÏÑ£ÕÒÓÔÖÝ×ØÜÞãßáâä¤èåæçé¥íêëìîóïñòô¦øõö÷ùýúûüþ§";

        public static string ConvertToVIQR(string InputString)
        {
            return Convert(InputString, "UNICODE", "VIQR");
        }

        public static string UnicodeToTCVN3(string value)
        {
            if (isCP1258(value))
                return Convert(CP1258ToUnicode(value), "UNICODE", "TCVN3");
            else if (isComposeUni(value))
                return Convert(ComposeUniToUnicode(value), "UNICODE", "TCVN3");
            else
                return Convert(value, "UNICODE", "TCVN3");
        }

        public static string TCVN3ToUnicode(string value)
        {
            char[] convertTable;
            char[] tcvnchars = {
                'µ', '¸', '¶', '·', '¹',
                '¨', '»', '¾', '¼', '½', 'Æ',
                '©', 'Ç', 'Ê', 'È', 'É', 'Ë',
                '®', 'Ì', 'Ð', 'Î', 'Ï', 'Ñ',
                'ª', 'Ò', 'Õ', 'Ó', 'Ô', 'Ö',
                '×', 'Ý', 'Ø', 'Ü', 'Þ',
                'ß', 'ã', 'á', 'â', 'ä',
                '«', 'å', 'è', 'æ', 'ç', 'é',
                '¬', 'ê', 'í', 'ë', 'ì', 'î',
                'ï', 'ó', 'ñ', 'ò', 'ô',
                '­', 'õ', 'ø', 'ö', '÷', 'ù',
                'ú', 'ý', 'û', 'ü', 'þ',
                '¡', '¢', '§', '£', '¤', '¥', '¦'
            };

            char[] unichars = {
                'à', 'á', 'ả', 'ã', 'ạ',
                'ă', 'ằ', 'ắ', 'ẳ', 'ẵ', 'ặ',
                'â', 'ầ', 'ấ', 'ẩ', 'ẫ', 'ậ',
                'đ', 'è', 'é', 'ẻ', 'ẽ', 'ẹ',
                'ê', 'ề', 'ế', 'ể', 'ễ', 'ệ',
                'ì', 'í', 'ỉ', 'ĩ', 'ị',
                'ò', 'ó', 'ỏ', 'õ', 'ọ',
                'ô', 'ồ', 'ố', 'ổ', 'ỗ', 'ộ',
                'ơ', 'ờ', 'ớ', 'ở', 'ỡ', 'ợ',
                'ù', 'ú', 'ủ', 'ũ', 'ụ',
                'ư', 'ừ', 'ứ', 'ử', 'ữ', 'ự',
                'ỳ', 'ý', 'ỷ', 'ỹ', 'ỵ',
                'Ă', 'Â', 'Đ', 'Ê', 'Ô', 'Ơ', 'Ư'
            };

            convertTable = new char[256];

            for (int i = 0; i < 256; i++)
                convertTable[i] = (char)i;

            for (int i = 0; i < tcvnchars.Length; i++)
                convertTable[tcvnchars[i]] = unichars[i];

            char[] chars = value.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
                if (chars[i] < (char)256)
                    chars[i] = convertTable[chars[i]];
            return new string(chars);
        }
        //public static string TCVN3ToUnicode(string value)
        //{
        //    return Convert(value, "TCVN3", "UNICODE");
        //}

        public static string CP1258ToUnicode(string value)
        {
            return Convert(value, "CP1258", "UNICODE");
        }

        public static string ComposeUniToUnicode(string value)
        {
            return Convert(value, "ComposeUni", "UNICODE");
        }
        public static bool isCP1258(string value)
        {
            return (value.ToLower().Contains("aÌ")
                 || value.ToLower().Contains("aÒ")
                 || value.ToLower().Contains("ãÌ")
                 || value.ToLower().Contains("ãÒ")
                 || value.ToLower().Contains("âì")
                 || value.ToLower().Contains("âò")
                 || value.ToLower().Contains("iÌ")
                 || value.ToLower().Contains("iÒ")
                 || value.ToLower().Contains("iò")
                 || value.ToLower().Contains("ôÞ")
                 || value.ToLower().Contains("ôò")
                 || value.ToLower().Contains("ð")
                 || value.ToLower().Contains("êÞ")
                 || value.ToLower().Contains("êì")
                 || value.ToLower().Contains("êÒ")
                 || value.ToLower().Contains("uÞ")
                 || value.ToLower().Contains("ýì")
                 || value.ToLower().Contains("ýò")
                );
        }

        public static bool isComposeUni(string value)
        {
            return (value.ToLower().Contains("ạ")
                 || value.ToLower().Contains("ấ")
                 || value.ToLower().Contains("ầ")
                 || value.ToLower().Contains("ậ")
                 || value.ToLower().Contains("ằ")
                 || value.ToLower().Contains("ị")
                 || value.ToLower().Contains("ì")
                 || value.ToLower().Contains("ỗ")
                 || value.ToLower().Contains("ặ")
                 || value.ToLower().Contains("ễ")
                 || value.ToLower().Contains("ộ")
                 || value.ToLower().Contains("ề")
                 || value.ToLower().Contains("ế")
                 || value.ToLower().Contains("ệ")
                 || value.ToLower().Contains("ể")
                 || value.ToLower().Contains("ũ")
                 || value.ToLower().Contains("ứ")
                 || value.ToLower().Contains("ự")
                );
        }
        public static string Convert(string InputString, string SourceCharMap, string DestinationCharMap)
        {
            string retVal = "";
            StringBuilder sb = new StringBuilder();
            int Pos = 0;
            int l = InputString.Length;
            int i = 0;
            Regex r;
            Match m;
            if (SourceCharMap != "VIQR" && SourceCharMap != "CP1258" && SourceCharMap != "ComposeUni")
            {
                for (i = 0; i < l; i++)
                {
                    Pos = -1;
                    switch (SourceCharMap)
                    {
                        case "UNICODE":
                            Pos = UnicodeMap.IndexOf(InputString[i].ToString());
                            break;
                        case "TCVN3":
                            r = new Regex(InputString[i].ToString());
                            m = r.Match(TCVN3Map);
                            if (m.Success)
                            {
                                Pos = m.Index;
                            }
                            break;
                    }

                    if (Pos < 0)
                    {
                        sb.Append(InputString[i]);
                    }
                    else
                    {
                        switch (DestinationCharMap)
                        {
                            case "UNICODE":
                                sb.Append(UnicodeMap[Pos]);
                                break;
                            case "CP1258":
                                sb.Append(CP1258Map[Pos]);
                                break;
                            case "VIQR":
                                sb.Append(VIQRMap[Pos]);
                                break;
                            case "TCVN3":
                                sb.Append(TCVN3Map[Pos]);
                                break;
                        }
                    }
                }
                retVal = sb.ToString();
            }
            else if (SourceCharMap == "CP1258")
            {
                retVal = InputString;
                l = CP1258MapReg.Length;
                for (i = 0; i < l; i = i + 2)
                {
                    r = new Regex(CP1258MapReg[i]);
                    retVal = r.Replace(retVal, CP1258MapReg[i + 1]);
                }
            }
            else if (SourceCharMap == "ComposeUni")
            {
                retVal = InputString;
                l = UComposeMapReg.Length;
                for (i = 0; i < l; i = i + 2)
                {
                    r = new Regex(UComposeMapReg[i]);
                    retVal = r.Replace(retVal, UComposeMapReg[i + 1]);
                }
            }
            else
            {
                retVal = InputString;
                l = VIQRMapReg.Length;
                for (i = 0; i < l; i = i + 2)
                {
                    r = new Regex(VIQRMapReg[i]);
                    retVal = r.Replace(retVal, VIQRMapReg[i + 1]);
                }
            }
            return retVal;
        }
    }

    public static class Common
    {
        public static string convertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        public static string ToTitleText(string phrase)
        {
            var rx = new Regex(@"(?<=\w)\w");
            return rx.Replace(phrase.ToUpper(), new MatchEvaluator(m => m.Value.ToLowerInvariant()));
        }
        public static Int32 ToInt32(object obj)
        {
            string sValue = "";
            if (obj != null)
                sValue = obj.ToString();
            Int32 retVal = 0;

            NumberFormatInfo numFormat = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat;
            //
            sValue = sValue.Replace(numFormat.NumberGroupSeparator, "");

            if (!Int32.TryParse(sValue, out retVal))
                retVal = 0;
            return retVal;
        }

        public static Int64 ToInt64(object obj)
        {
            string sValue = "";
            if (obj != null)
                sValue = obj.ToString();
            Int64 retVal = 0;

            NumberFormatInfo numFormat = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat;
            //
            sValue = sValue.Replace(numFormat.NumberGroupSeparator, "");

            if (!Int64.TryParse(sValue, out retVal))
                retVal = 0;
            return retVal;
        }

        public static Decimal ToDecimal(object obj)
        {
            string sValue = "";
            if (obj != null)
                sValue = obj.ToString();

            Decimal retVal = 0;

            NumberFormatInfo numFormat = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat;
            //
            sValue = sValue.Replace(numFormat.NumberGroupSeparator, "");

            if (!Decimal.TryParse(sValue, out retVal))
                retVal = 0;
            return retVal;
        }

        public static Double ToDouble(object obj)
        {
            string sValue = "";
            if (obj != null)
                sValue = obj.ToString();

            Double retVal = 0;

            NumberFormatInfo numFormat = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat;
            //
            sValue = sValue.Replace(numFormat.NumberGroupSeparator, "");

            if (!Double.TryParse(sValue, out retVal))
                retVal = 0;
            return retVal;
        }

        public static DateTime ToDateTime(object obj)
        {
            string sValue = "";
            if (obj != null)
                sValue = obj.ToString();
            sValue = sValue.Trim();

            System.DateTime mDate = DateTime.MinValue;
            if (System.DateTime.TryParseExact(sValue, "dd/MM/yyyy", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal, out mDate))
            {
                return mDate;
            }
            else if (System.DateTime.TryParse(sValue, out mDate))
            {
                return mDate;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        public static string Md5(string sInput)
        {
            HashAlgorithm algorithmType = default(HashAlgorithm);
            ASCIIEncoding enCoder = new ASCIIEncoding();
            byte[] valueByteArr = enCoder.GetBytes(sInput);
            byte[] hashArray = null;
            // Encrypt Input string 
            algorithmType = new MD5CryptoServiceProvider();
            hashArray = algorithmType.ComputeHash(valueByteArr);
            //Convert byte hash to HEX
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashArray)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        public static string Encrypt(string plainText)
        {
            string key = "bshc@2212#";
            byte[] EncryptKey = { };
            byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
            EncryptKey = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByte = Encoding.UTF8.GetBytes(plainText);
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, des.CreateEncryptor(EncryptKey, IV), CryptoStreamMode.Write);
            cStream.Write(inputByte, 0, inputByte.Length);
            cStream.FlushFinalBlock();
            return HttpServerUtility.UrlTokenEncode(mStream.ToArray());
            //return Convert.ToBase64String();
        }

        public static string Decrypt(string encryptedText)
        {
            string key = "bshc@2212#";
            byte[] DecryptKey = { };
            byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
            byte[] inputByte = new byte[encryptedText.Length];

            DecryptKey = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByte = HttpServerUtility.UrlTokenDecode(encryptedText);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(DecryptKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByte, 0, inputByte.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}