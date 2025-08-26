namespace BongoCat_Like
{
    public static class AutorunManager
    {
        public static void Add(string name, string file)
        {
            GlobalHelper.Config.Autorun = true;
        }

        public static void Remove(string name)
        {
            GlobalHelper.Config.Autorun = false;
        }
    }
}
