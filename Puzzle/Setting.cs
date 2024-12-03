using Puzzle.Models;
using System;
using System.Linq.Expressions;

namespace Puzzle
{
    public static class Setting
    {
        public const string Kavenegar = "5478344D543073485A33376C513472484A3059534C7976332B5A565855504E7A6B674B5367664C794659513D";

        public static bool IsDevelopment { get; set; } = true;

        public static int Print(DateTime dateTime)
        {
            //int print = 30;

            //if (dateTime < new DateTime(2021, 03, 04))
            //{
            //    print = 5;
            //}
            //else if (dateTime < new DateTime(2023, 04, 03))
            //{
            //    print = 10;
            //}
            //else if (dateTime < new DateTime(2023, 10, 10))
            //{
            //    print = 20;
            //}

            int print = 45;

            return print;
        }

        public static string ToEnNumber(this string obj)
        {
            if (string.IsNullOrWhiteSpace(obj))
            {
                return null;
            }

            obj = obj.Replace("۱", "1");
            obj = obj.Replace("۲", "2");
            obj = obj.Replace("۳", "3");
            obj = obj.Replace("۴", "4");
            obj = obj.Replace("۵", "5");
            obj = obj.Replace("۶", "6");
            obj = obj.Replace("۷", "7");
            obj = obj.Replace("۸", "8");
            obj = obj.Replace("۹", "9");
            obj = obj.Replace("۰", "0");

            obj = obj.Replace("١", "1");
            obj = obj.Replace("٢", "2");
            obj = obj.Replace("٣", "3");
            obj = obj.Replace("٤", "4");
            obj = obj.Replace("٥", "5");
            obj = obj.Replace("٦", "6");
            obj = obj.Replace("٧", "7");
            obj = obj.Replace("٨", "8");
            obj = obj.Replace("٩", "9");
            obj = obj.Replace("٠", "0");

            obj = obj.Trim();

            return obj;
        }

        //      public static Func<Conversation, bool> showOrHideForCustomer =  x =>x.IsDelete == false&& (x.Accepted == true ||
        //(x.Type == Enums.Enum.MediaType.Render && (x.Project.App || x.Project.Whatsapp || x.Project.Telegram || x.Project.PrintDelivery || x.Project.Eitaa || x.Project.Soroush || x.Project.Rubika || x.Project.Other || x.Project.Flash)) ||
        //(x.Type == Enums.Enum.MediaType.Screen && x.Project.Screenshot));
    }
}
