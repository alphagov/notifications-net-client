using Newtonsoft.Json;

namespace Notify.Models
{
    public class CostDetails
    {
        [JsonProperty("billable_sms_fragments")]
        public int? billableSmsFragments { get; set; }

        [JsonProperty("international_rate_multiplier")]
        public double? internationalRateMultiplier { get; set; }

        [JsonProperty("sms_rate")]
        public double? smsRate { get; set; }

        [JsonProperty("billable_sheets_of_paper")]
        public int? billableSheetsOfPaper { get; set; }

        public string postage { get; set; }

        public override bool Equals(object costDetail)
        {
            if (!(costDetail is CostDetails cd))
            {
                return false;
            }

            return billableSmsFragments == cd.billableSmsFragments &&
                   internationalRateMultiplier == cd.internationalRateMultiplier &&
                   smsRate == cd.smsRate &&
                   billableSheetsOfPaper == cd.billableSheetsOfPaper &&
                   postage == cd.postage;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
