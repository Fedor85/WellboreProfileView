using System;
using WellboreProfileView.Enums;

namespace WellboreProfileView
{
    public static class EnumsHelper
    {
        public static string GetDisplayPageRegionTypeName(DisplayPageRegionType displayPageRegionType)
        {
            switch (displayPageRegionType)
            {
                case DisplayPageRegionType.NoN:
                    return "Неопредлено";
                case DisplayPageRegionType.OneTable:
                    return "Таблица";
                case DisplayPageRegionType.MultiTableText:
                    return "Данные по траектории";
                case DisplayPageRegionType.MultiTableProfile:
                    return "Отобразить профиль";
                case DisplayPageRegionType.MultiTablePlan:
                    return "Отобразить план";
                case DisplayPageRegionType.MultiTableMultiDraw:
                    return "Отобразить профиль/план";
                case DisplayPageRegionType.MultiTable3D:
                    return "3D";
                default:
                    throw new ArgumentException(String.Format("Не задано имя для {0}", displayPageRegionType));
            }
        }

        public static long GetNextMultiTablePositionTypeId(long multiTablePositionTypeId)
        {
            if (multiTablePositionTypeId == (long)MultiTablePositionType.Down)
                return (long)MultiTablePositionType.Up;

            return (long)MultiTablePositionType.Down;
        }

        public static bool IsMultiTable(long displayPageRegionTypeId)
        {
            return displayPageRegionTypeId == (long)DisplayPageRegionType.MultiTableText ||
                   displayPageRegionTypeId == (long)DisplayPageRegionType.MultiTableProfile ||
                   displayPageRegionTypeId == (long)DisplayPageRegionType.MultiTablePlan ||
                   displayPageRegionTypeId == (long)DisplayPageRegionType.MultiTableMultiDraw ||
                   displayPageRegionTypeId == (long)DisplayPageRegionType.MultiTable3D;
        }
    }
}