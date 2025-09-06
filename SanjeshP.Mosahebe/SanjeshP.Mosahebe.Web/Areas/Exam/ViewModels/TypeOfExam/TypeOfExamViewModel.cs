using SanjeshP.Mosahebe.Common.MyAttribute;
using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.ComponentModel.DataAnnotations;

namespace SanjeshP.Mosahebe.Web.Areas.Exam.ViewModels.TypeOfExam
{
    public class TypeOfExamViewModel : BaseDto<TypeOfExamViewModel, SanjeshP.Mosahebe.Entities.Exam.TypeOfExam, Guid>
    {
        public Guid TypeOfExamId { get; set; }

        public int StandardCode { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }

}
