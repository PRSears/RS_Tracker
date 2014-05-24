using System.Data.Linq;

namespace RS_Goal_Tracker.Data
{
    public class HighscoresDataContext : DataContext
    {
        public Table<Player> Users;
        public Table<Skill> Scores;

        public HighscoresDataContext(string ConnectionString):base(ConnectionString)
        {
            // might need to do some logic-ing
        }
    }
}
