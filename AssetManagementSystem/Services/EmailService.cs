using AssetManagementSystem.DTO;
using AssetManagementSystem.Services;

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;


namespace AssetManagementSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string message,
            string signature)
        {
            // 1) Build the MIME message
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                // HTML body (optional)
                HtmlBody = $"<p>{message}</p><p>— {signature}</p>",
                // Plain-text fallback
                TextBody = $"{message}\n\n— {signature}"
            };
            email.Body = bodyBuilder.ToMessageBody();

            // 2) Connect & authenticate to SMTP
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.ServerCertificateValidationCallback = static (sender, certificate, chain, sslPolicyErrors) => true;

            await smtp.ConnectAsync(
                _settings.SmtpHost,
                _settings.SmtpPort,
                _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

            if (!string.IsNullOrWhiteSpace(_settings.SmtpUser))
            {
                await smtp.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
            }

            // 3) Send & disconnect
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
    // Add this method to your EmailService or create a separate EmailTemplateService
    public class EmailTemplateService
    {
        public static string GetAssignmentEmailTemplate(string employeeName, DateTime assignmentDate,
            string make, string model, string serialNumber, string jobTitle, string comments)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            background-color: #2c5aa0;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 5px 5px 0 0;
        }}
        .content {{
            background-color: #f9f9f9;
            padding: 25px;
            border: 1px solid #ddd;
            border-top: none;
        }}
        .footer {{
            background-color: #2c5aa0;
            color: white;
            padding: 15px;
            text-align: center;
            border-radius: 0 0 5px 5px;
            font-size: 12px;
        }}
        .asset-details {{
            background-color: white;
            border: 1px solid #ddd;
            border-radius: 5px;
            padding: 15px;
            margin: 15px 0;
        }}
        .detail-row {{
            margin-bottom: 8px;
            padding: 5px 0;
        }}
        .label {{
            font-weight: bold;
            color: #2c5aa0;
            min-width: 150px;
            display: inline-block;
        }}
        .divider {{
            border-top: 2px solid #2c5aa0;
            margin: 20px 0;
        }}
        .signature {{
            margin-top: 25px;
            padding-top: 15px;
            border-top: 1px solid #ddd;
        }}
    </style>
</head>
<body>
    <div class='header'>
        <h2>🏢 Asset Assignment Notification</h2>
    </div>
    
    <div class='content'>
        <p>Dear <strong>{employeeName}</strong>,</p>
        
        <p>You have been assigned a company asset on <strong>{assignmentDate:dddd, MMMM dd, yyyy 'at' h:mm:ss tt}</strong>.</p>
        
        <div class='asset-details'>
            <h3 style='color: #2c5aa0; margin-top: 0;'>📦 Asset Details</h3>
            
            <div class='detail-row'>
                <span class='label'>Make:</span>
                <span>{make}</span>
            </div>
            
            <div class='detail-row'>
                <span class='label'>Model:</span>
                <span>{model}</span>
            </div>
            
            <div class='detail-row'>
                <span class='label'>Serial Number:</span>
                <span>{serialNumber}</span>
            </div>
        </div>

        <div class='asset-details'>
            <h3 style='color: #2c5aa0; margin-top: 0;'>👤 Employee Information</h3>
            
            <div class='detail-row'>
                <span class='label'>Position:</span>
                <span>{jobTitle}</span>
            </div>
            
            <div class='detail-row'>
                <span class='label'>Assignment Date:</span>
                <span>{assignmentDate:MMMM dd, yyyy}</span>
            </div>
        </div>

        {(string.IsNullOrEmpty(comments) ? "" : $@"
        <div class='asset-details'>
            <h3 style='color: #2c5aa0; margin-top: 0;'>💬 Additional Remarks</h3>
            <p>{comments}</p>
        </div>")}

        <div class='divider'></div>

        <div style='background-color: #e8f4fd; padding: 15px; border-radius: 5px; border-left: 4px solid #2c5aa0;'>
            <h4 style='color: #2c5aa0; margin-top: 0;'>📋 Important Notes</h4>
            <ul style='margin-bottom: 0;'>
                <li>This asset remains the property of CIDRZ</li>
                <li>Please handle the equipment with care and report any issues immediately</li>
                <li>All assets must be returned upon termination of employment</li>
                <li>Unauthorized transfer or modification is prohibited</li>
            </ul>
        </div>

        <div class='signature'>
            <p>Best regards,</p>
            <p><strong>ICT Department</strong><br>
            Centre for Infectious Disease Research in Zambia (CIDRZ)<br>
            📧 ICT Support: support@cidrz.org<br>
            📞 Contact: +260 977 830083</p>
        </div>
    </div>
    
    <div class='footer'>
        <p>CONFIDENTIAL: This email and any attachments are confidential and intended solely for the use of the individual to whom they are addressed.</p>
    </div>
</body>
</html>";
        }

        public static string GetBorrowEmailTemplate(string employeeName, DateTime borrowDate,
            string category, string make, string model, string serialNumber, string jobTitle,
            DateTime dateFrom, DateTime dateTo)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        /* Same CSS as above */
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            background-color: #2c5aa0;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 5px 5px 0 0;
        }}
        .content {{
            background-color: #f9f9f9;
            padding: 25px;
            border: 1px solid #ddd;
            border-top: none;
        }}
        .footer {{
            background-color: #2c5aa0;
            color: white;
            padding: 15px;
            text-align: center;
            border-radius: 0 0 5px 5px;
            font-size: 12px;
        }}
        .asset-details {{
            background-color: white;
            border: 1px solid #ddd;
            border-radius: 5px;
            padding: 15px;
            margin: 15px 0;
        }}
        .detail-row {{
            margin-bottom: 8px;
            padding: 5px 0;
        }}
        .label {{
            font-weight: bold;
            color: #2c5aa0;
            min-width: 150px;
            display: inline-block;
        }}
        .divider {{
            border-top: 2px solid #2c5aa0;
            margin: 20px 0;
        }}
        .signature {{
            margin-top: 25px;
            padding-top: 15px;
            border-top: 1px solid #ddd;
        }}
        .urgent {{
            background-color: #fff3cd;
            border: 1px solid #ffeaa7;
            border-radius: 5px;
            padding: 15px;
            margin: 15px 0;
        }}
    </style>
</head>
<body>
    <div class='header'>
        <h2>📅 Asset Borrow Notification</h2>
    </div>
    
    <div class='content'>
        <p>Dear <strong>{employeeName}</strong>,</p>
        
        <p>You have borrowed a company asset on <strong>{borrowDate:dddd, MMMM dd, yyyy 'at' h:mm:ss tt}</strong>.</p>
        
        <div class='asset-details'>
            <h3 style='color: #2c5aa0; margin-top: 0;'>📦 Asset Details</h3>
            
            <div class='detail-row'>
                <span class='label'>Category:</span>
                <span>{category}</span>
            </div>
            
            <div class='detail-row'>
                <span class='label'>Make:</span>
                <span>{make}</span>
            </div>
            
            <div class='detail-row'>
                <span class='label'>Model:</span>
                <span>{model}</span>
            </div>
            
            <div class='detail-row'>
                <span class='label'>Serial Number:</span>
                <span>{serialNumber}</span>
            </div>
        </div>

        <div class='asset-details'>
            <h3 style='color: #2c5aa0; margin-top: 0;'>📋 Borrow Period</h3>
            
            <div class='detail-row'>
                <span class='label'>Borrow Date:</span>
                <span>{dateFrom:dddd, MMMM dd, yyyy}</span>
            </div>
            
            <div class='detail-row'>
                <span class='label'>Return Date:</span>
                <span style='color: #d63031; font-weight: bold;'>{dateTo:dddd, MMMM dd, yyyy}</span>
            </div>
            
            <div class='detail-row'>
                <span class='label'>Duration:</span>
                <span>{(dateTo - dateFrom).TotalDays} days</span>
            </div>
        </div>

        <div class='urgent'>
            <h4 style='color: #e17055; margin-top: 0;'>⚠️ Important Reminder</h4>
            <p>Please ensure this asset is returned by <strong>{dateTo:MMMM dd, yyyy}</strong>. Late returns may affect asset availability for other staff members.</p>
        </div>

        <div class='divider'></div>

        <div style='background-color: #e8f4fd; padding: 15px; border-radius: 5px; border-left: 4px solid #2c5aa0;'>
            <h4 style='color: #2c5aa0; margin-top: 0;'>🔒 Asset Responsibility</h4>
            <ul style='margin-bottom: 0;'>
                <li>You are responsible for the safekeeping of this asset</li>
                <li>Report any damage or issues immediately to ICT</li>
                <li>Do not install unauthorized software</li>
                <li>Return the asset in the same condition as received</li>
            </ul>
        </div>

        <div class='signature'>
            <p>Best regards,</p>
            <p><strong>ICT Department</strong><br>
            Centre for Infectious Disease Research in Zambia (CIDRZ)<br>
           📧 ICT Support: support@cidrz.org<br>
            📞 Contact: +260 977 830083</p>
        </div>
    </div>
    
    <div class='footer'>
        <p>CONFIDENTIAL: This email and any attachments are confidential and intended solely for the use of the individual to whom they are addressed.</p>
    </div>
</body>
</html>";
        }

        public static string GetMaintenanceEmailTemplate(string employeeName, DateTime submissionDate,
            string category, string make, string model, string serialNumber, string ticket,
            string diagnosis, DateTime returnDate)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        /* Same CSS as above */
    </style>
</head>
<body>
    <div class='header'>
        <h2>🔧 Maintenance Request Confirmation</h2>
    </div>
    
    <div class='content'>
        <p>Dear <strong>{employeeName}</strong>,</p>
        
        <p>Your maintenance request has been successfully submitted on <strong>{submissionDate:dddd, MMMM dd, yyyy 'at' h:mm:ss tt}</strong>.</p>
        
        <div class='asset-details'>
            <h3 style='color: #2c5aa0; margin-top: 0;'>📦 Asset Information</h3>
            
            <div class='detail-row'>
                <span class='label'>Category:</span>
                <span>{category}</span>
            </div>
            
            <div class='detail-row'>
                <span class='label'>Make:</span>
                <span>{make}</span>
            </div>
            
            <div class='detail-row'>
                <span class='label'>Model:</span>
                <span>{model}</span>
            </div>
            
            <div class='detail-row'>
                <span class='label'>Serial Number:</span>
                <span>{serialNumber}</span>
            </div>
        </div>

        <div class='asset-details'>
            <h3 style='color: #2c5aa0; margin-top: 0;'>🔧 Maintenance Details</h3>
            
            <div class='detail-row'>
                <span class='label'>Ticket Number:</span>
                <span style='font-weight: bold; color: #2c5aa0;'>{ticket}</span>
            </div>
            
            <div class='detail-row'>
                <span class='label'>Issue Description:</span>
                <span>{diagnosis}</span>
            </div>
            
            <div class='detail-row'>
                <span class='label'>Submission Date:</span>
                <span>{submissionDate:MMMM dd, yyyy}</span>
            </div>
            
            <div class='detail-row'>
                <span class='label'>Expected Return:</span>
                <span style='color: #00b894; font-weight: bold;'>{returnDate:dddd, MMMM dd, yyyy}</span>
            </div>
        </div>

        <div class='divider'></div>

        <div style='background-color: #f0f8ff; padding: 15px; border-radius: 5px; border-left: 4px solid #00b894;'>
            <h4 style='color: #00b894; margin-top: 0;'>📞 Next Steps</h4>
            <ul style='margin-bottom: 0;'>
                <li>Our technical team will review your request within 24 hours</li>
                <li>You will receive updates on the repair progress</li>
                <li>Contact ICT if you have urgent questions about your request</li>
                <li>We will notify you when the asset is ready for collection</li>
            </ul>
        </div>

        <div class='signature'>
            <p>Best regards,</p>
            <p><strong>ICT Department</strong><br>
            Centre for Infectious Disease Research in Zambia (CIDRZ)<br>
            📧 ICT Support: support@cidrz.org<br>
            📞 Contact: +260 977 830083</p>
        </div>
    </div>
    
    <div class='footer'>
        <p>CONFIDENTIAL: This email and any attachments are confidential and intended solely for the use of the individual to whom they are addressed.</p>
    </div>
</body>
</html>";
        }
    }
}
