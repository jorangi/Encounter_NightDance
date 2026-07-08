namespace Encounter.NightDance.Core
{
    public static class CameraService
    {
        private static IFieldObject cameraController;
        public static IFieldObject CameraController
        {
            get => cameraController;
        }
        public static void Register(CameraController _c)
        {
            cameraController ??= _c;
        }
    }
}