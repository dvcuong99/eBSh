using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using static eBSH.Models.GiftCodePaginatedVM;

namespace eBSH.Models
{
    public class GiftCode
    {
        public GiftCode()
        {
            CodeKM = "";
            TLKM = 0;
            OrderId = 0;
            CTBH = "";
            MTN = "";
        }
        [Required(ErrorMessage = "Nhập mã khuyến mại")]
        public string CodeKM { get; set; }
        [Required(ErrorMessage = "Nhập chương trình bảo hiểm")]
        public string CTBH { get; set; }
        public string MTN { get; set; }
        [Required(ErrorMessage = "Thông tin tỷ lệ khuyến mại của mã chưa nhập")]
        [Range(1, 100, ErrorMessage = "Tỷ lệ khuyến mại từ 1 đến 100")]
        public int TLKM { get; set; }
        public decimal OrderId { get; set; }
        public string Ma_Dvi { get; set; }
        [Required(ErrorMessage = "Nhập mã bảo hiểm")]
        public string Ma_BH { get; set; }
        public string Ma_DL { get; set; }
        public string Ma_NQL { get; set; }
        public string Ma_NSD { get; set; }
        [DateLessThan("EndDate", ErrorMessage = "Ngày bắt đầu hiệu lực trước ngày kết thúc")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Nhập ngày hiệu lực của thẻ khuyến mại")]
        public DateTime EndDate { get; set; }
        public string KENH_KT { get; set; }
        [Required(ErrorMessage = "Thông tin số lượng mã khuyến mại chưa nhập")]
        [Range(1,Int32.MaxValue,ErrorMessage ="số lượng mã khuyến mại không hợp lệ")]
        public decimal AmountKM { get; set; }
        public decimal RemainderKM { get; set; }
    }

    // So sánh 2 datetime
    public class DateLessThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateLessThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                ErrorMessage = ErrorMessageString;
                var currentValue = (DateTime)value;

                var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

                if (property == null)
                    throw new ArgumentException("Property with this name not found");

                var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

                if (currentValue > comparisonValue)
                    return new ValidationResult(ErrorMessage);

                return ValidationResult.Success;
            }
            return ValidationResult.Success;

        }
    }
    public class DateTimeType : ValidationAttribute, IClientValidatable
    {
        public DateTimeType()
        {
            ErrorMessage = "Invalid Date Time";
        }

        public DateTimeType(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public override bool IsValid(object value)
        {
            //This is already done by model binding
            DateTime minDateTime;
            DateTime.TryParse((value ?? string.Empty).ToString(), out minDateTime);

            return minDateTime != DateTime.MinValue;
        }

        // this adds client side rule vaidation
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string datePattern = @"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$";//yyyy/mm/dd or yyyy-mm-dd
            yield return new ModelClientValidationRegexRule(ErrorMessage, datePattern);
        }
    }
    public class GiftCodePaginatedVM
    {
        public IEnumerable<GiftCode> GiftCodeList { get; set; }
        public int ItemPerPage { get; set; }
        public int CurrentPage { get; set; }

        public int PageCount()
        {
            return Convert.ToInt32(Math.Ceiling(GiftCodeList.Count() / (double)ItemPerPage));
        }
        public IEnumerable<GiftCode> PaginatedList()
        {
            int start = (CurrentPage - 1) * ItemPerPage;
            return GiftCodeList.OrderByDescending(b => b.StartDate).Skip(start).Take(ItemPerPage);
        }

    }
}