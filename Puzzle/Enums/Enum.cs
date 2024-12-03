using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Enums
{
    public static class Enum
    {
        public enum SmsTemplate
        {
            PuzzleAutoVerify
        }

        public enum Role
        {
            Admin,
            Designer,
            Customer
        }

        public enum Gender
        {
            Male = 1,
            Female = 0
        }

        public enum SMSCategory
        {
            BeforeBirthday = 1,
            Birthday = 2,
            BackToShopping1 = 11,
            BackToShopping2 = 12,
            BackToShopping3 = 13,
            BackToShopping4 = 14,
            CompleteProject = 3
        }

        public enum DiscountCategory
        {
            Birthday = 1,
            BackToShopping = 2,
            Volume = 3
        }

        public enum ProjectCategory
        {
            [Description("کابینت مدرن")]
            ModernCabinet = 0,

            [Description("کابینت کلاسیک")]
            ClassicCabinet = 1,

            [Description("وی ری")]
            Vray = 2,

            [Description("دکور زیر 100")]
            DecorBelow100 = 3,

            [Description("دکور 100 تا 500")]
            DecorBetween100To500 = 4,

            [Description("دکور بالای 500")]
            DecorOver500 = 5,

            [Description("دکور 100 تا 300")]
            DecorBetween100To300 = 6,

            [Description("دکور 300 تا 500")]
            DecorBetween300To500 = 7,

            [Description("منبت")]
            Inlay = 8,

            [Description("تغییرات")]
            Edit = 9,

            [Description("اصلاحیه")]
            Amendment = 10,

            [Description("پرینت")]
            Print = 11,

            [Description("مدرن پلاس")]
            PlusModernCabinet = 12,

            [Description("کلاسیک پلاس")]
            PlusClassicCabinet = 13,

            [Description("دکور")]
            Decor = 14,

            [Description("سایر")]
            Other = 15,

            [Description("مدرن وی ری")]
            ModernVray = 16,

            [Description("کلاسیک وی ری")]
            ClassicVray = 17,

            [Description("دکور وی ری")]
            DecorVray = 18,

            [Description("دو بعدی")]
            TwoDimensional = 19,


            [Description("کابینت مدرن")]
            Cabinet_Modern = 100,

            [Description("کابینت کلاسیک")]
            Cabinet_Classic = 101,

            [Description("کابینت نئوکلاسیک")]
            Cabinet_NeoClassic = 102,

            [Description("کابینت انزو")]
            Cabinet_Enzo = 103,

            [Description("کابینت سایر")]
            Cabinet_Other = 104,

            [Description("انواع کمد")]
            Decor_Closet = 105,

            [Description("دکوراسیون داخلی کامل")]
            Decor_Full = 106,

            //[Description("دکوراسیون داخلی - نئوکلاسیک")]
            //Decor_NeoClassic = 107,

            //[Description("دکوراسیون داخلی - انزو")]
            //Decor_Enzo = 108,

            [Description("دکوراسیون داخلی سایر")]
            Decor_Other = 109,

            [Description("طراحی محیط تجاری و اداری")]
            Official = 110,

            //[Description("طراحی محیط تجاری و اداری - کلاسیک")]
            //Official_Classic = 111,

            //[Description("طراحی محیط تجاری و اداری - نئوکلاسیک")]
            //Official_NeoClassic = 112,

            //[Description("طراحی محیط تجاری و اداری - انزو")]
            //Official_Enzo = 113,

            //[Description("طراحی محیط تجاری و اداری - سایر")]
            //Official_Other = 114,

            [Description("نمای ساختمان و محوطه")]
            Facade = 115,

            [Description("سی ان سی دو بعدی و لیزر")]
            CNC = 116,

            [Description("سی ان سی سه بعدی و منبت")]
            CNC_Inlaid = 117,

            //[Description("CNC و دوبعدی - نئوکلاسیک")]
            //CNC_NeoClassic = 118,

            [Description("طراحی صنعتی")]
            CNC_Industrial = 119,

            [Description("پلن دو بعدی")]
            CNC_Plan = 120,

            [Description("CNC و دو بعدی - سایر")]
            CNC_Other = 121,

            [Description("موشن گرافی")]
            MotionGraphics = 122,

            [Description("طراحی لوگو")]
            Logo = 123,

            [Description("کنسل شده")]
            Cancel = 200,
        }

        public enum PaymentType
        {
            [Description("نقدی")]
            Cash = 0,

            [Description("پوز سامان")]
            SamanCart = 1,

            [Description("پوز ملت")]
            MellatCart = 2,

            [Description("واریز به سامان")]
            DepositToSaman = 3,

            [Description("واریز به ملت")]
            DepositToMellat = 4,

            [Description("واریز به پاسارگاد")]
            DepositToPasargad = 5,

            [Description("برداشت از اعتبار")]
            FromCredit = 6,

            [Description("اصلاح سند")]
            NewFound = 7,

            [Description("پس دادن به مشتری")]
            Return = 8,

            [Description("انتقال به اعتبار")]
            ToCredit = 9,

            [Description("واریز به طراح")]
            ToDesigner = 10
        }

        public enum OrderType
        {
            Office = 0,
            Whatsapp = 1,
            Telegram = 2,
            Online = 3,
            Other = 4,
            Soroush = 5,
            Eitaa = 6,
            Rubika = 7,
            App = 8,
        }

        public enum MediaType
        {
            Screen = 0,
            Render = 1,
            Image = 2,
            Voice = 3,
            File = 4,
            Payment
        }

        public enum Status
        {
            [Display(Name = "جدید")]
            New = 1,

            [Display(Name = "ویرایش شده")]
            Edit = 2,

            [Display(Name = "حذف شده")]
            Delete = 3,

            [Display(Name = "عدم نمایش")]
            Disable = 4,

            [Display(Name = "تایید")]
            Verify = 5,

            [Display(Name = "خطا در پرداخت")]
            Error = 6,

            [Display(Name = "پرداخت شده")]
            Pay = 7,

            [Display(Name = "پرداخت نشده")]
            NoPay = 8,

            [Display(Name = "تکمیل شده")]
            Complete = 9,

            [Display(Name = "کنسل شده")]
            Cancel = 10
        }

        public enum MediaStatus
        {
            New = 0,
            Read = 1
        }

        public enum OrderBy
        {
            Alphabet = 1,
            LastVisitTime = 2,
            NumberOfProject = 3,
            LastMonthRating = 4,
            Debtor = 5,
            Creditor = 6,
            Critical = 7,
            Special = 8,
            LastActivity = 9,
            Balance = 10,
            Credit = 11,
            Support = 12
        }

        public enum Delivery
        {
            notsync,
            send,
            pending,
            failed,
            discarded,
            delivered
        }

        public enum DeliveryType
        {
            Print = 0,
            Whatsapp = 1,
            Telegram = 2,
            Other = 3,
            Screenshot = 4,
            Notification = 5,
            Ready = 6,
            Follow = 7,
            Soroush = 8,
            Eitaa = 9,
            Rubika = 10,
            MediaDone = 11,
            Cancel = 12,
            Flash = 13,
            App = 14,
            Archive = 15
        }

        public enum ProjectProperty
        {
            TotalPrice = 1,
            TotalPayment = 2,
            DesignerPercent = 3
        }

        public enum CustomerStatus
        {
            None = 0,
            Good = 1,
            Bad = 2
        }

        public enum EntityStatus
        {
            Add = 0,
            Update = 1,
            Remove = 2
        }

        public enum Quality
        {
            [Description("استاندارد")]
            Standard,

            [Description("VIP")]
            VIP,

            [Description("انیمیشن")]
            Animation
        }

        public enum ProjectReportReason
        {
            [Description("تا ظهر آماده می شود")]
            UntilNoon = 1,

            [Description("تا غروب آماده می شود")]
            UntilEvening = 2,

            [Description("منتظر تایید اسکرین شات هستم")]
            Scrennshot = 3,

            [Description("مشتری پاسخگو نیست")]
            NoAnswar = 4,

            [Description("بیعانه یا مبلغ پروژه واریز نشده است")]
            NoMoney = 5,

            [Description("اطلاعات پروژه ناقص می باشد")]
            Incomplete = 6,

            [Description("تاریخ تحویل اشتباه ثبت شده است")]
            MistakeDelivery = 7,

            [Description("")]
            Other = 8
        }

        public enum ImageFormat
        {
            Bmp,
            Emf,
            Exif,
            Gif,
            Icon,
            Jpeg,
            MemoryBmp,
            Png,
            Tiff,
            Wmf
        }
    }
}
