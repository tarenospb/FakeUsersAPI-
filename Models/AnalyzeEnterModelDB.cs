using System;

namespace FakeUsersAPI.Models
{
    public class AnalyzeEnterModelDB
    {
            public DateTime Date { get; set; }
            public Guid IdUser { get; set; }
            public string? IPLocate { get; set; }
            public string? IPReason { get; set; }
            public string? PassChange { get; set; }
            public string? PassReason { get; set; }
            public string? InCorrectLogin { get; set; }
            public string? BrowserFail { get; set; }
            public string? OSFail { get; set; }
            public bool Valid { get; set; }

        public AnalyzeEnterModelDB() 
        {
            Date = new DateTime();
            IdUser = new Guid();
            IPLocate = "correct";
            IPReason = " ";
            PassChange = "correct";
            PassReason = " ";
            InCorrectLogin = "correct";
            BrowserFail = "correct";
            OSFail = "correct";
            Valid = true;
        }
        public override String ToString()
        {
            String result = "";
            result += "Date of enter analyze: " + this.Date + "\n";
            result += "Locate by IP is " + this.IPLocate + "\n";
            result += "Reason: " + this.IPReason + "\n";
            result += "Autority by passport is " + this.PassChange + "\n";
            result += "Reason: " + this.PassReason + "\n";
            result += "Email is " + this.InCorrectLogin + "\n";
            result += "Last browser is " + this.BrowserFail + "\n";
            result += "Last OS is " + this.OSFail + "\n";
            result += "Enter is valid: " + this.Valid;
            return result;
        }

    }
}
