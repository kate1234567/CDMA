using System;
using System.Linq;
using System.Text;

namespace CDMA
{
    class Program
    {
        static int[][] WalshCodes;

        static string[] Messages = new string[] { "GOD", "CAT", "HAM", "SUN" };

        static void Main(string[] args)
        {
            WalshCodes = GenerateWalshCodes(8);

            var binaryMessages = Messages.Select(ConvertToBinary).ToArray();

            var encodedMessages = new int[Messages.Length][];
            for (int i = 0; i < Messages.Length; i++)
            {
                encodedMessages[i] = EncodeMessage(binaryMessages[i], WalshCodes[i]);
                Console.WriteLine($"Станция {i + 1} ({Messages[i]}) закодировала сообщение: {string.Join(", ", encodedMessages[i])}");
            }

            var combinedSignal = CombineSignals(encodedMessages);
            Console.WriteLine($"Скомбинированный сигнал на канале:\n{string.Join(", ", combinedSignal)}");

            for (int i = 0; i < Messages.Length; i++)
            {
                var decodedMessageBinary = DecodeMessage(combinedSignal, WalshCodes[i], binaryMessages[i].Length);
                var decodedMessage = ConvertFromBinary(decodedMessageBinary);
                Console.WriteLine($"Станция {i + 1} передает: {decodedMessage}");
            }
        }

        static int[][] GenerateWalshCodes(int n)
        {
            int[][] codes = new int[n][];
            for (int i = 0; i < n; i++)
            {
                codes[i] = new int[n];
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    codes[i][j] = (CountBits(i & j) % 2 == 0) ? 1 : -1;
                }
            }
            return codes;
        }

        static int CountBits(int value)
        {
            int count = 0;
            while (value > 0)
            {
                count += value & 1;
                value >>= 1;
            }

            return count;
        }

        static int[] ConvertToBinary(string message)
        {
            StringBuilder binary = new StringBuilder();
            foreach (char c in message)
            {
                binary.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return binary.ToString().Select(b => b == '1' ? 1 : -1).ToArray();
        }

        static string ConvertFromBinary(int[] binaryMessage)
        {
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < binaryMessage.Length; i += 8)
            {
                string byteString = string.Join("", binaryMessage.Skip(i).Take(8).Select(b => b == 1 ? '1' : '0'));
                text.Append((char)Convert.ToInt32(byteString, 2));
            }

            return text.ToString();
        }

        static int[] EncodeMessage(int[] binaryMessage, int[] walshCode)
        {
            int[] encodedMessage = new int[binaryMessage.Length * walshCode.Length];
            for (int i = 0; i < binaryMessage.Length; i++)
            {
                for (int j = 0; j < walshCode.Length; j++)
                {
                    encodedMessage[i * walshCode.Length + j] = binaryMessage[i] * walshCode[j];
                }
            }

            return encodedMessage;
        }

        static int[] CombineSignals(int[][] encodedMessages)
        {
            int length = encodedMessages[0].Length;
            int[] combinedSignal = new int[length];
            for (int i = 0; i < length; i++)
            {
                combinedSignal[i] = encodedMessages.Sum(em => em[i]);
            }

            return combinedSignal;
        }

        static int[] DecodeMessage(int[] combinedSignal, int[] walshCode, int messageLength)
        {
            int[] decodedMessage = new int[messageLength];
            for (int i = 0; i < messageLength; i++)
            {
                int sum = 0;
                for (int j = 0; j < walshCode.Length; j++)
                {
                    sum += combinedSignal[i * walshCode.Length + j] * walshCode[j];
                }

                decodedMessage[i] = sum / walshCode.Length > 0 ? 1 : -1;
            }

            return decodedMessage;
        }
    }
}
