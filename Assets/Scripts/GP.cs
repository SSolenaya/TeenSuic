namespace Assets.Scripts {
    public static class SceneNames {
        public static string mainSceneName = "startScreen";
        public static string gameSceneName = "game";
    }

    public class GP {
        
        public const int portionOfMeds = 3;     //  макс порция таблеток, выходящая на экран одновременно
        public const int totalAmountMeds = 100; //  общее число таблеток за сессию
        public const float timeBetweenPortions = 2f; //  время между появлением порций
        public const int lives = 100;           //  количество жизней
        public const string stateOfSound = "State of sound";
    }
}