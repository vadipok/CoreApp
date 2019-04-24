using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kern.Models
{
    public class Approximation
    {
        public double[] XArr { get; set; }

        public double[] YArr { get; set; }

        public double a { get; set; }
        public double b { get; set; }
        public double R2 { get; set; }

        // Экспоненциальная аппроксимация 
        public void Exponential()
        {
            // проверки
            PersonException();

            //+++++++++++++++++++++++++++++++++++++++++++
            // расчеты для нахождения a и b
            //+++++++++++++++++++++++++++++++++++++++++++
            // количество элементов в множествах
            int N = XArr.Length;
            // сумма логарифмов YArr
            double sumLogYArr = 0;
            // сумма квадратов XArr
            double sumSquareXArr = 0;
            // сумма логарифмов XArr
            double sumLogXArr = 0;
            // сумма логарифма YArr умноженная на XArr
            double sumLogYArr_XArr = 0;
            // сумма XArr
            double sumXArr = XArr.Sum();

            // заполнение промежуточных данных
            for (int i = 0; i < N; i++)
            {
                sumLogYArr += Math.Log(YArr[i]);
                sumSquareXArr += XArr[i] * XArr[i];
                sumLogXArr += Math.Log(XArr[i]);
                sumLogYArr_XArr += Math.Log(YArr[i]) * XArr[i];
            }

            // расчет коэффициента а
            double log_a = (sumLogYArr * sumSquareXArr - sumLogYArr_XArr * sumXArr) / (N * sumSquareXArr - sumXArr * sumXArr);
            a = Math.Exp(log_a);

            // расчет коэффициента b
            b = (N * sumLogYArr_XArr - sumLogYArr * sumXArr) / (N * sumSquareXArr - sumXArr * sumXArr);

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // расчет коэффициента достоверности
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // значения, полученные через функцию апроксимации
            double[] Fi = new double[N];
            for (int i = 0; i < N; i++)
            {
                Fi[i] = a * Math.Exp(XArr[i] * b);
            }

            // Среднее значение по фактам
            double avgYArr = YArr.Average();

            // расчет числителя и знаменателя коэффициента достоверности
            double SSres = 0;
            double SStot = 0;
            double RSS = 0;
            double ESS = 0;
            double term3 = 0;
            for (int i = 0; i < N; i++)
            {
                SSres += (YArr[i] - Fi[i]) * (YArr[i] - Fi[i]);
                SStot += (YArr[i] - avgYArr) * (YArr[i] - avgYArr);
                RSS += (YArr[i] - Fi[i]) * (YArr[i] - Fi[i]);
                ESS += (Fi[i] - avgYArr) * (Fi[i] - avgYArr);
                term3 += 2 * (YArr[i] - Fi[i]) * (Fi[i] - avgYArr);
            }
            // расчет коэффициента достоверности
            var TSS = RSS + ESS;
            var R2wi = 1 - ESS / TSS; // model with an intercept
            var R2woi = 1 - (ESS + term3) / (TSS + term3); // model without an intercept

            R2 = 1 - (SSres / SStot);
        }

        private void PersonException()
        {
            if (XArr.Length != YArr.Length)
            {
                throw new Exception("Количество элементов в множествах X и Y должно быть одинаковым");
            }

            if (XArr.Length < 2 || YArr.Length < 2)
            {
                throw new Exception("Количество элементов в множествах X и Y должно быть больше одного");
            }
        }

    }
}
