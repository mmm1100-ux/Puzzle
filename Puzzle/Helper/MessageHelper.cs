using Models;
using Puzzle.Models;
using System;
using System.Linq;
using static Enums.Enum;

namespace Helper
{
    public static class MessageHelper
    {
        public static string ToLog(this History history)
        {
            string message = string.Empty;

            if (history.Property.Any())
            {
                var type = history.Property.Select(c => c.Type);

                if (type.Contains(ProjectProperty.TotalPayment) && type.Contains(ProjectProperty.TotalPrice))
                {
                    message = "مبلغ کل و مجموع مبلغ دریافتی پروژه بروزرسانی شد!";
                }
                else if (type.Contains(ProjectProperty.TotalPayment))
                {
                    message = "مجموع مبلغ دریافتی پروژه بروزرسانی شد!";
                }
                else if (type.Contains(ProjectProperty.TotalPrice))
                {
                    message = "مبلغ کل پروژه بروزرسانی شد!";
                }
                else if (type.Contains(ProjectProperty.DesignerPercent))
                {
                    message = "درصد طراح بروزرسانی شد!";
                }
            }
            else
            {
                message = "پروژه بروزرسانی شد!";
            }

            return message;
        }
    }
}
