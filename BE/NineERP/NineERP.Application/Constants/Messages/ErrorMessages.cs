namespace NineERP.Application.Constants.Messages
{
    public static class ErrorMessages
    {
        public static string GetMessage(string code)
        {
            return code switch
            {
                "DP001" => "Vui lòng nhập URL.",
                "DP002" => "URL tối thiểu 3 ký tự.",
                "DP003" => "URL tối đa 100 ký tự.",
                "DP004" => "URL không hợp lệ. Không được chứa dấu hoặc ký tự đặc biệt.",
                "DP005" => "Tên phòng ban là bắt buộc.",
                "DP006" => "Tên phòng ban quá ngắn (tối thiểu 2 ký tự).",
                "DP007" => "Tên phòng ban quá dài (tối đa 255 ký tự).",
                "DP008" => "Email là bắt buộc.",
                "DP009" => "Email không hợp lệ.",
                "DP010" => "Email không được vượt quá 255 ký tự.",
                "DP011" => "Số điện thoại là bắt buộc.",
                "DP012" => "Số điện thoại chỉ chứa số, từ 9 đến 15 chữ số.",
                "DP013" => "Địa chỉ chính là bắt buộc.",
                "DP014" => "Địa chỉ quá ngắn.",
                "DP015" => "Địa chỉ không được vượt quá 255 ký tự.",
                "DP016" => "Địa chỉ phụ không được vượt quá 255 ký tự.",
                "DP017" => "Link Facebook không vượt quá 255 ký tự.",
                "DP018" => "Đường dẫn Facebook không hợp lệ.",
                "DP019" => "Link Wikipedia không vượt quá 255 ký tự.",
                "DP020" => "Đường dẫn Wikipedia không hợp lệ.",
                "DP021" => "Link Youtube không vượt quá 255 ký tự.",
                "DP022" => "Đường dẫn Youtube không hợp lệ.",
                _ => "Lỗi không xác định"
            };
        }
    }
}
