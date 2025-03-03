

namespace SnakePowerByte.Events
{
    public class EventService
    {
        public EventController<int> OnMapSelected { get; private set; }
        public EventController<int> OnWaveStart { get; private set; }
        public EventController<bool> OnGameOver { get; private set; }
        public EventService()
        {
            OnMapSelected = new EventController<int>();
            OnWaveStart = new EventController<int>();
            OnGameOver = new EventController<bool>();
        }

    }
}