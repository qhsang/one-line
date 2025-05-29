// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("hFaze85GT+DQ47IuyhJeCmNC3UYBVUY+F1irmJi60CeyrRwgUhTU+Afuf6vybpU+7vCzRPSZlHisdvRV3oFJsANw5TUp6qUBwyIoSC7bUr8FwtaYopyttdulqyPBXongr0hDlE/Mws39T8zHz0/MzM0uBitC8wNADExY3hPDeUeyfdDskyjJnM+S9fEYstjlsdf684F73xjhDAPFAoO2SyMOAiXLGVdXEjsRFsNItYJKLwCX/U/M7/3Ay8TnS4VLOsDMzMzIzc7xQrdiK1ALuHpzxt0kg1VSbv1ZrCLUrEQ2/ayakyfPt6r8UhKWJ6YOk+NVR4iQIjDqKNKQyKaE1ZtOl0cqHRjDf8EbeppiwaHz5pEXCjM6uqIMBFgV4X+IOs/OzM3M");
        private static int[] order = new int[] { 13,5,6,9,10,5,12,9,11,13,11,13,13,13,14 };
        private static int key = 205;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
