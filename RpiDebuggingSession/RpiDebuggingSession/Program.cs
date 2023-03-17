using global::RaspberryPiNetCore.ADC;
using global::RaspberryPiNetCore.JoySticks;
using global::RaspberryPiNetCore.LCD;
using global::RaspberryPiNetCore.TWIST;
using RpiDebuggingSession;
using System;
using System.Diagnostics;

namespace Raspberry_Pi_Dot_Net_Core_Console_Application3
{
    internal class Program
    {
        private static ADC1015 ADC = new ADC1015();
        public static SerLCD LCD = new SerLCD();
        static void Main(string[] args)
        {
        ConvertToPercentage prc = new ConvertToPercentage();

            byte signal = 0;
            var outputRead = ADC.ReadADC_SingleEnded(signal);
            var value = ADC.SINGLE_Measurement[signal].Take();
            LCD.lcdDisplay();
            LCD.lcdPrint(value.ToString());
            
            var date = DateTime.Now;
            
            LCD.lcdClear();
            LCD.lcdHome();
            LCD.lcdPrint(date.ToString());
            LCD.lcdGotoXY(0, 1);
            LCD.lcdPrint(value.ToString());
            LCD.lcdGotoXY(0, 2);

            var converted = prc.ConvertTo100(value);

            LCD.lcdPrint(converted.ToString()+"%");
        }
    }
}
