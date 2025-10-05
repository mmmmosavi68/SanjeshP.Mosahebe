using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Web.Areas.DocumentVerification.ViewModels
{
    public class SubmissionDataInputViewModel
    {
        public int SubmissionDataId { get; set; }
        public int? FormItemDefinitionId { get; set; }
        public int? FormItemCoefficientDefinitionId { get; set; }

        public bool? IsApproved { get; set; }
        public int? ManualScore { get; set; }
        public string RejectReason { get; set; }
    }
}
