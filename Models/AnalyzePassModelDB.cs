using System;

namespace FakeUsersAPI.Models
{
    public class AnalyzePassModelDB
    {
            public DateTime Date { get; set; }
            public Guid IdUser { get; set; }
            public string? Serie { get; set; }
            public string? Number { get; set; }
            public string? SerieEqualRegion { get; set; }

            public string? Code { get; set; }
            public string? PassIsOut { get; set; }
            public bool Valid { get; set; }

        public AnalyzePassModelDB() 
        {
            Date = new DateTime();
            IdUser = new Guid();
            Serie = "correct";
            Number = "correct";
            SerieEqualRegion = "correct";
            Code = "correct";
            PassIsOut = "correct";
            Valid = true;
        }
        public override String ToString()
        {
            String result = "";
            result += "Date of analyze: " + this.Date + "\n";
            result += "Number: " + this.Number + "\n";
            result += "Serie: " + this.Serie + "\n";
            result += "Serie equal to region issue: " + this.SerieEqualRegion + "\n";
            result += "Code: " + this.Code + "\n";
            result += "Pass is out: " + this.PassIsOut + "\n";
            result += "Pass is valid: " + this.Valid;
            return result;
        }

    }
}
