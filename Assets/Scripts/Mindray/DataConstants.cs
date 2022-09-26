namespace VSCaptureMRay
{
    public class DataConstants
    {
        public static byte[] query_request_bytes1 = {
        0x4d, 0x53, 0x48, 0x7c, 0x5e, 0x7e, 0x26, 0x7c, 0x7c, 0x7c, 0x7c, 0x7c, 0x7c, 0x7c, 0x51, 0x52,
        0x59, 0x5e, 0x52, 0x30, 0x32, 0x7c, 0x31, 0x32, 0x30, 0x33, 0x7c, 0x50, 0x7c, 0x32, 0x2e, 0x33,
        0x2e, 0x31, 0x3c, 0x43, 0x52, 0x3e, 0x20, 0x51, 0x52, 0x44, 0x7c, 0x32, 0x30, 0x30, 0x36, 0x30,
        0x37, 0x33, 0x31, 0x31, 0x34, 0x35, 0x35, 0x35, 0x37, 0x7c, 0x52, 0x7c, 0x49, 0x7c, 0x51, 0x38,
        0x39, 0x35, 0x32, 0x31, 0x31, 0x7c, 0x7c, 0x7c, 0x7c, 0x7c, 0x52, 0x45, 0x53, 0x3c, 0x43, 0x52,
        0x3e, 0x20, 0x51, 0x52, 0x46, 0x7c, 0x4d, 0x4f, 0x4e, 0x7c, 0x7c, 0x7c, 0x7c, 0x30, 0x26, 0x30,
        0x5e, 0x31, 0x5e, 0x31, 0x5e, 0x30, 0x5e, 0x31, 0x30, 0x31, 0x26, 0x31, 0x30, 0x32, 0x3c, 0x43,
        0x52, 0x3e  };

        public static string query_request_string1 = "MSH|^~\\&|||||||QRY^R02|1203|P|2.3.1<CR> QRD|20060731145557|R|I|Q895211|||||RES<CR> QRF|MON||||0&0^1^1^0^101&102&103&104<CR>";

        public static string query_request_string2 = "MSH|^~\\&|||||||QRY^R02|1203|P|2.3.1\rQRD|20060731145557|R|I|Q895211|||||RES\rQRF|MON||||0&0^1^1^0^101&102&103&104\r";

        public static string query_request_string3 = "MSH|^~\\&|||||||QRY^R02|1203|P|2.3.1\rQRD|19970731145557|R|I|Q839572|||||RES\rQRF|MON||||3232241478&5^1^1^0^101&102&103&104\rQRF|MON||||3232241478&5^1^1^0^151&160&200\r";

        public static string query_request_string4 = "MSH|^~\\&|||||||QRY^R02|1203|P|2.3.1\rQRD|19970731145557|R|I|Q839572|||||RES\rQRF|MON||||3232241478&5^1^1^1^101&102&103&104\rQRF|MON||||3232241478&5^1^1^1^151&160&200\r";

        public static string query_request_string5 = "MSH|^~\\&|||||||QRY^R02|1203|P|2.3.1\rQRD|20060731145557|R|I|Q895211|||||RES\rQRF|MON||||0&0^1^1^1^101&102&103&104\r";

        public static string query_request_string6 = "MSH|^~\\&|||||||QRY^R02|1203|P|2.3.1\rQRD|20060731145557|R|I|Q895211|||||RES\rQRF|MON||||0&0^1^1^0^101\r";

        public static string query_request_string7 = "MSH|^~\\&|||||||QRY^R02|1203|P|2.3.1\rQRD|20060731145557|R|I|Q895211|||||RES\rQRF|MON||||0&0^1^1^0^101&500&501&502\r";

        public static string query_request_string8 = "MSH|^~\\&|||||||QRY^R02|1203|P|2.3.1\rQRD|20060731145557|R|I|Q895211|||||RES\rQRF|MON||||0&0^1^1^0^101&500&501&502\rQRF|MON||||0&0^1^1^0^160&161&220&222\rQRF|MON||||0&0^1^1^0^170&171&172&151\rQRF|MON||||0&0^1^1^0^503&504&505\rQRF|MON||||0&0^1^1^0^200&201&202&203\r";

        public static string query_request_string9 = "MSH|^~\\&|||||||QRY^R02|2|P|2.3.1\rQRD|20060731145557|R|I|Q895211|||||RES\rQRF|MON||||0&0^1^1^0^101\r";

        public static string ACK_string = "MSH|^~\\&|Mindray|Gateway|||||ACK|3|P|2.3.1\rMSA|AA|Q002\r";

        public static string tcp_echo_msg1 = "MSH|^~\\&|||||||ORU^R01|106|P|2.3.1|";
    }
}
