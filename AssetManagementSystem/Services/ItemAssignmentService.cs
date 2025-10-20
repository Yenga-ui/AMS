namespace AssetManagementSystem.Services
{
    public class ItemAssignmentService
    {
        private readonly IEmailService _emailService;

        public ItemAssignmentService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task AssignItemAsync(String serial,String make,String model,String category, string employeeEmail, String?employeeName)
        {
            // Assignment logic here...

            var subject = "New Item Assigned";
            var body = "<p>You have been assigned a new item.<br/>"+"Type: "+category+"<br/>Make: "+make+"<br/"+make+
                
                "<br/>model: "+model+"<br/>serial: "+serial+".</p>";

            await _emailService.SendEmailAsync(employeeEmail, subject, body,"AMS");
        }
    }

}
