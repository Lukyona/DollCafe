// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Bkj/eh6ZTFM4RK611zXbj7ObvlZ6yEtoekdMQ2DMAsy9R0tLS09KScj1DrBBzkfVY2AOc0RTbrxCINmEIWKzOPoKY6IF86HL9SWTFr10aKktL0YYP9C8z7xyhcEpqQVCeCV+s88kmwMToJxcCYfQZhT/f2OPVtiDQTDOE/JPlCDU41l46HfhGuhKdKbZW5J5PUeoIdJ2AF4oMfRXgkRCtIGliA28HG4CXKbDFRRFGqN07DgsxrHpVxJjz5cqLV4qzmidYYV2kiWm9G4QOe6BKnPcaSsacILsLPqH4apumpXoHUcoET4L1Xi0E7yTlXC9yEtFSnrIS0BIyEtLStAhZ8UQGD+rb+7F6q7hlUuaqJes8VC0eZIQpLvtkKxyDnjwyUhJS0pL");
        private static int[] order = new int[] { 13,13,3,5,8,7,12,12,9,10,12,12,13,13,14 };
        private static int key = 74;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
