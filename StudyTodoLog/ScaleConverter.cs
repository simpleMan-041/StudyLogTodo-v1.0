using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace StudyTodoLog
{
    public class ScaleConverter : IValueConverter
    {
        // ウィンドウ幅に応じてコントロールのサイズをスケーリングする
        public double BaseValue { get; set; } = 28; // コントロールにおける基準値
        public double BaseWidth { get; set; } = 1000; // ウィンドウサイズ基準値
        public double Min { get; set; } = 14;  // 過度な縮小を防ぐ最小値
        public double Max { get; set; } = 42;  // 過度な拡大を防ぐ最大値  

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double actualWidth) return BaseValue;

            double scale = actualWidth / BaseWidth;
            double result = scale * BaseValue;

            if (result < Min) result = Min;
            if (result > Max) result = Max;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
