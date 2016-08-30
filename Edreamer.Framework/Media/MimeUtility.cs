﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Edreamer.Framework.Collections;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Media
{
    public static class MimeUtility
    {
        [DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
        static extern int FindMimeFromData(
            IntPtr pBC,
            [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)] byte[] pBuffer,
            int cbSize,
            [MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
            int dwMimeFlags,
            out IntPtr ppwzMimeOut,
            int dwReserved);

        // ToDo [10281807]: Use another library to detect Mime type. urlmon's FindMimeFromData is too limited.
        /// <summary>
        /// Detects the content Mime type 
        /// </summary>
        /// <param name="data">The data to detect its Mime type</param>
        /// <param name="name">[Optional] Name or path of the content to detect its Mime type</param>
        /// <returns>Returns the content Mime type</returns>
        public static string GetMimeType(byte[] data, string name = null)
        {
            Throw.IfArgumentNull(data, "data");
            var maxContent = Math.Min(data.Length, 256);
            var buf = new byte[maxContent];
            Buffer.BlockCopy(data, 0, buf, 0, maxContent);
            IntPtr mimeout;
            var result = FindMimeFromData(IntPtr.Zero, name, buf, maxContent, null, 0, out mimeout, 0);
            if (result != 0)
                throw Marshal.GetExceptionForHR(result);
            var mime = Marshal.PtrToStringUni(mimeout);
            Marshal.FreeCoTaskMem(mimeout);
            return mime;
        }

        /// <summary>
        /// Determines the suitable extension for a Mime type 
        /// </summary>
        /// <param name="mimeType">The Mime type</param>
        /// <returns>Returns the extension</returns>
        public static string GetExtension(string mimeType)
        {
            Throw.IfArgumentNullOrEmpty(mimeType, "mimeType");
            var extension = ExtensionsMap
                .Where(m =>  m.MimeType.EqualsIgnoreCase(mimeType))
                .Select(m => m.Extension)
                .FirstOrDefault();
            return extension;
        }


        public readonly static IEnumerable<MimeExtension> ExtensionsMap = new TupleList<string>
        {
            { "dwg", "application/acad" },
            { "ez", "application/andrew-inset" },
            { "ccad", "application/clariscad" },
            { "drw", "application/drafting" },
            { "tsp", "application/dsptype" },
            { "dxf", "application/dxf" },
            { "evy", "application/envoy" },
            { "fif", "application/fractals" },
            { "spl", "application/futuresplash" },
            { "hta", "application/hta" },
            { "unv", "application/i-deas" },
            { "acx", "application/internet-property-stream" },
            { "jar", "application/java-archive" },
            { "hqx", "application/mac-binhex40" },
            { "cpt", "application/mac-compactpro" },
            { "doc", "application/msword" },
            { "dot", "application/msword" },
            { "bin", "application/octet-stream" },
            { "class", "application/octet-stream" },
            { "dms", "application/octet-stream" },
            { "exe", "application/octet-stream" },
            { "lha", "application/octet-stream" },
            { "lzh", "application/octet-stream" },
            { "oda", "application/oda" },
            { "ogg", "application/ogg" },
            { "ogm", "application/ogg" },
            { "ods", "application/oleobject" },
            { "axs", "application/olescript" },
            { "pdf", "application/pdf" },
            { "pgp", "application/pgp" },
            { "prf", "application/pics-rules" },
            { "p10", "application/pkcs10" },
            { "p7c", "application/pkcs7-mime" },
            { "p7m", "application/pkcs7-mime" },
            { "p7s", "application/pkcs7-signature" },
            { "crl", "application/pkix-crl" },
            { "ai", "application/postscript" },
            { "eps", "application/postscript" },
            { "ps", "application/postscript" },
            { "prt", "application/pro_eng" },
            { "rtf", "application/rtf" },
            { "set", "application/set" },
            { "setpay", "application/set-payment-initiation" },
            { "setreg", "application/set-registration-initiation" },
            { "stl", "application/SLA" },
            { "smi", "application/smil" },
            { "smil", "application/smil" },
            { "sol", "application/solids" },
            { "step", "application/STEP" },
            { "stp", "application/STEP" },
            { "vda", "application/vda" },
            { "mif", "application/vnd.mif" },
            { "xls", "application/vnd.ms-excel" },
            { "xla", "application/vnd.ms-excel" },
            { "xlc", "application/vnd.ms-excel" },
            { "xll", "application/vnd.ms-excel" },
            { "xlm", "application/vnd.ms-excel" },
            { "xlt", "application/vnd.ms-excel" },
            { "xlw", "application/vnd.ms-excel" },
            { "cat", "application/vnd.ms-pkiseccat" },
            { "ppt", "application/vnd.ms-powerpoint" },
            { "pot", "application/vnd.ms-powerpoint" },
            { "pps", "application/vnd.ms-powerpoint" },
            { "ppz", "application/vnd.ms-powerpoint" },
            { "mpp", "application/vnd.ms-project" },
            { "wcm", "application/vnd.ms-works" },
            { "wdb", "application/vnd.ms-works" },
            { "wks", "application/vnd.ms-works" },
            { "wps", "application/vnd.ms-works" },
            { "pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { "cod", "application/vnd.rim.cod" },
            { "sst", "application/vndms-pkicertstore" },
            { "pko", "application/vndms-pkipko" },
            { "cat", "application/vndms-pkiseccat" },
            { "stl", "application/vndms-pkistl" },
            { "hlp", "application/winhlp" },
            { "arj", "application/x-arj-compressed" },
            { "bcpio", "application/x-bcpio" },
            { "cdf", "application/x-cdf" },
            { "vcd", "application/x-cdlink" },
            { "pgn", "application/x-chess-pgn" },
            { "z", "application/x-compress" },
            { "tgz", "application/x-compressed" },
            { "cpio", "application/x-cpio" },
            { "csh", "application/x-csh" },
            { "deb", "application/x-debian-package" },
            { "dcr", "application/x-director" },
            { "dir", "application/x-director" },
            { "dxr", "application/x-director" },
            { "dvi", "application/x-dvi" },
            { "pre", "application/x-freelance" },
            { "spl", "application/x-futuresplash" },
            { "gtar", "application/x-gtar" },
            { "gz", "application/x-gzip" },
            { "hdf", "application/x-hdf" },
            { "ins", "application/x-internet-signup" },
            { "isp", "application/x-internet-signup" },
            { "iii", "application/x-iphone" },
            { "ipx", "application/x-ipix" },
            { "ips", "application/x-ipscript" },
            { "js", "application/x-javascript" },
            { "skd", "application/x-koan" },
            { "skm", "application/x-koan" },
            { "skp", "application/x-koan" },
            { "skt", "application/x-koan" },
            { "latex", "application/x-latex" },
            { "lsp", "application/x-lisp" },
            { "scm", "application/x-lotusscreencam" },
            { "mdb", "application/x-msaccess" },
            { "crd", "application/x-mscardfile" },
            { "clp", "application/x-msclip" },
            { "bat", "application/x-msdos-program" },
            { "com", "application/x-msdos-program" },
            { "dll", "application/x-msdownload" },
            { "m13", "application/x-msmediaview" },
            { "m14", "application/x-msmediaview" },
            { "mvb", "application/x-msmediaview" },
            { "wmf", "application/x-msmetafile" },
            { "mny", "application/x-msmoney" },
            { "pub", "application/x-mspublisher" },
            { "scd", "application/x-msschedule" },
            { "trm", "application/x-msterminal" },
            { "wri", "application/x-mswrite" },
            { "nc", "application/x-netcdf" },
            { "pma", "application/x-perfmon" },
            { "pmc", "application/x-perfmon" },
            { "pml", "application/x-perfmon" },
            { "pmr", "application/x-perfmon" },
            { "pmw", "application/x-perfmon" },
            { "pl", "application/x-perl" },
            { "pm", "application/x-perl" },
            { "p12", "application/x-pkcs12" },
            { "pfx", "application/x-pkcs12" },
            { "p7b", "application/x-pkcs7-certificates" },
            { "spc", "application/x-pkcs7-certificates" },
            { "p7r", "application/x-pkcs7-certreqresp" },
            { "p7c", "application/x-pkcs7-mime" },
            { "p7m", "application/x-pkcs7-mime" },
            { "p7s", "application/x-pkcs7-signature" },
            { "rar", "application/x-rar-compressed" },
            { "sh", "application/x-sh" },
            { "shar", "application/x-shar" },
            { "swf", "application/x-shockwave-flash" },
            { "sit", "application/x-stuffit" },
            { "sv4cpio", "application/x-sv4cpio" },
            { "sv4crc", "application/x-sv4crc" },
            { "tar", "application/x-tar" },
            { "tar.gz", "application/x-tar-gz" },
            { "tgz", "application/x-tar-gz" },
            { "tcl", "application/x-tcl" },
            { "tex", "application/x-tex" },
            { "texi", "application/x-texinfo" },
            { "texinfo", "application/x-texinfo" },
            { "roff", "application/x-troff" },
            { "t", "application/x-troff" },
            { "tr", "application/x-troff" },
            { "man", "application/x-troff-man" },
            { "me", "application/x-troff-me" },
            { "ms", "application/x-troff-ms" },
            { "ustar", "application/x-ustar" },
            { "src", "application/x-wais-source" },
            { "sst", "application/x-wais-source" },
            { "cer", "application/x-x509-ca-cert" },
            { "crt", "application/x-x509-ca-cert" },
            { "der", "application/x-x509-ca-cert" },
            { "zip", "application/x-zip-compressed" },
            { "pko", "application/ynd.ms-pkipko" },
            { "zip", "application/zip" },
            { "aiff", "audio/aiff" },
            { "aifc", "audio/aiff" },
            { "au", "audio/basic" },
            { "snd", "audio/basic" },
            { "rmi", "audio/mid" },
            { "mid", "audio/mid" },
            { "kar", "audio/midi" },
            { "mid", "audio/midi" },
            { "midi", "audio/midi" },
            { "mp3", "audio/mpeg" },
            { "mpga", "audio/mpeg" },
            { "tsi", "audio/TSP-audio" },
            { "wav", "audio/wav" },
            { "aif", "audio/x-aiff" },
            { "aifc", "audio/x-aiff" },
            { "aiff", "audio/x-aiff" },
            { "m3u", "audio/x-mpegurl" },
            { "wax", "audio/x-ms-wax" },
            { "wma", "audio/x-ms-wma" },
            { "ram", "audio/x-pn-realaudio" },
            { "rm", "audio/x-pn-realaudio" },
            { "ra", "audio/x-pn-realaudio" },
            { "rpm", "audio/x-pn-realaudio-plugin" },
            { "ra", "audio/x-realaudio" },
            { "wav", "audio/x-wav" },
            { "pdb", "chemical/x-pdb" },
            { "xyz", "chemical/x-pdb" },
            { "bmp", "image/bmp" },
            { "dib", "image/bmp" },
            { "cod", "image/cis-cod" },
            { "gif", "image/gif" },
            { "ief", "image/ief" },
            { "jpg", "image/jpeg" },
            { "jpe", "image/jpeg" },
            { "jpeg", "image/jpeg" },
            { "jfif", "image/jpeg" },
            { "jfif", "image/pipeg" },
            { "jpg", "image/pjpeg" },
            { "jpe", "image/pjpeg" },
            { "jpeg", "image/pjpeg" },
            { "jfif", "image/pjpeg" },
            { "png", "image/png" },
            { "svg", "image/svg+xml" },
            { "tif", "image/tiff" },
            { "tiff", "image/tiff" },
            { "ras", "image/x-cmu-raster" },
            { "cmx", "image/x-cmx" },
            { "ico", "image/x-icon" },
            { "png", "image/x-png" },
            { "pnm", "image/x-portable-anymap" },
            { "pbm", "image/x-portable-bitmap" },
            { "pgm", "image/x-portable-graymap" },
            { "ppm", "image/x-portable-pixmap" },
            { "rgb", "image/x-rgb" },
            { "tif", "image/x-tiff" },
            { "tiff", "image/x-tiff" },
            { "xbm", "image/x-xbitmap" },
            { "xpm", "image/x-xpixmap" },
            { "xwd", "image/x-xwindowdump" },
            { "mht", "message/rfc822" },
            { "mhtml", "message/rfc822" },
            { "nws", "message/rfc822" },
            { "eml", "message/rfc822" },
            { "iges", "model/iges" },
            { "igs", "model/iges" },
            { "mesh", "model/mesh" },
            { "msh", "model/mesh" },
            { "silo", "model/mesh" },
            { "css", "text/css" },
            { "323", "text/h323" },
            { "htm", "text/html" },
            { "html", "text/html" },
            { "stm", "text/html" },
            { "uls", "text/iuls" },
            { "txt", "text/plain" },
            { "asc", "text/plain" },
            { "bas", "text/plain" },
            { "c", "text/plain" },
            { "cc", "text/plain" },
            { "f", "text/plain" },
            { "f90", "text/plain" },
            { "h", "text/plain" },
            { "hh", "text/plain" },
            { "m", "text/plain" },
            { "rtx", "text/richtext" },
            { "sct", "text/scriptlet" },
            { "sgm", "text/sgml" },
            { "sgml", "text/sgml" },
            { "tsv", "text/tab-separated-values" },
            { "jad", "text/vnd.sun.j2me.app-descriptor" },
            { "htt", "text/webviewhtml" },
            { "htc", "text/x-component" },
            { "xml", "text/xml" },
            { "disco", "text/xml" },
            { "wsdl", "text/xml" },
            { "xsl", "text/xml" },
            { "xsd", "text/xml" },
            { "etx", "text/x-setext" },
            { "vcf", "text/x-vcard" },
            { "dl", "video/dl" },
            { "fli", "video/fli" },
            { "flv", "video/flv" },
            { "gl", "video/gl" },
            { "mp4", "video/mp4" },
            { "mp2", "video/mpeg" },
            { "mpa", "video/mpeg" },
            { "mpe", "video/mpeg" },
            { "mpeg", "video/mpeg" },
            { "mpg", "video/mpeg" },
            { "mpv2", "video/mpeg" },
            { "m1v", "video/mpeg" },
            { "mov", "video/quicktime" },
            { "qt", "video/quicktime" },
            { "viv", "video/vnd.vivo" },
            { "vivo", "video/vnd.vivo" },
            { "ivf", "video/x-ivf" },
            { "lsf", "video/x-la-asf" },
            { "lsx", "video/x-la-asf" },
            { "asf", "video/x-ms-asf" },
            { "asr", "video/x-ms-asf" },
            { "asx", "video/x-ms-asf" },
            { "asx", "video/x-ms-asx" },
            { "avi", "video/x-msvideo" },
            { "wmv", "video/x-ms-wmv" },
            { "wmx", "video/x-ms-wmx" },
            { "wvx", "video/x-ms-wvx" },
            { "movie", "video/x-sgi-movie" },
            { "mime", "www/mime" },
            { "ice", "x-conference/x-cooltalk" },
            { "flr", "x-world/x-vrml" },
            { "vrm", "x-world/x-vrml" },
            { "vrml", "x-world/x-vrml" },
            { "wrl", "x-world/x-vrml" },
            { "wrz", "x-world/x-vrml" },
            { "xaf", "x-world/x-vrml" },
            { "xof", "x-world/x-vrml" }
        }.Select(x => new MimeExtension { Extension = x.Item1, MimeType = x.Item2 } );
    }

    public class MimeExtension
    {
        public string Extension { get; set; }
        public string MimeType { get; set; }
    }
}