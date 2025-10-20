﻿namespace AssetManagementSystem.DTO
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = "";
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; } = "";
        public string SmtpPass { get; set; } = "";
        public string FromName { get; set; } = "";
        public string FromEmail { get; set; } = "";
        public bool UseSsl { get; set; }
    }
}
