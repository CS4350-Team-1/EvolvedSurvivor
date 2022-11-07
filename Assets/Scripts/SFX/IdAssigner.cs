public class IdAssigner
{
    private static int nextId = 0;

    public static int GetSoundId()
    {
        nextId++;
        return nextId;
    }
}
