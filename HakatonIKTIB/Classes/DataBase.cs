using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace HakatonIKTIB.Classes
{
    public static class DataBase
    {
        static MySqlConnection Connection = new("server=bx7ffq9zspouu6xw4pmr-mysql.services.clever-cloud.com;port=3306;username=ukqtlwyhjiffhmh9;password=dpkcZW3mEFSKyYs5iNgW;database=bx7ffq9zspouu6xw4pmr");
        public static User? Login(string login, string pass)
        {
            MySqlCommand getUserCommand = new($"SELECT * FROM `Users` WHERE `Login`='{login}' AND `Password`='{pass}' ", Connection);
            MySqlDataAdapter getUser = new(getUserCommand);
            DataTable user = new();
            getUser.Fill(user);

            if (user.Rows.Count > 0)
            {
                ulong id = (ulong)user.Rows[0][0];
                string name = (string)user.Rows[0][3];
                string lastname = (string)user.Rows[0][4];
                string surname = (string)user.Rows[0][5];
                int course = (int)user.Rows[0][6];
                string specialization = (string)user.Rows[0][8];
                string university = (string)user.Rows[0][8];
                List<Tag> tags = GetTags(id, false);
                List<Discussion> activeDiscussions = GetDiscussionsByApplicantId(id, false);
                List<Discussion> closedDiscussions = GetDiscussionsByApplicantId(id, true);

                return new(id, login, pass, name, lastname, surname, course, specialization, university, tags, activeDiscussions, closedDiscussions);
            }
            else
            {
                return null;
            }
        }

        public static User? Register(string login, string pass, string name, string lastname, string surname, int course, string specialization, string univer, out string output)
        {
            MySqlCommand getUserCommand = new($"SELECT * FROM `Users` WHERE `Login`='{login}'", Connection);
            MySqlDataAdapter getUser = new(getUserCommand);
            DataTable user = new();
            getUser.Fill(user);

            if (user.Rows.Count > 0)
            {
                output = "Такой пользователь уже существует";
                return null;
            }

            else
            {
                MySqlCommand getIdCommand = new($"SELECT * FROM `Users`", Connection);
                MySqlDataAdapter getId = new(getIdCommand);
                DataTable idTable = new();
                getId.Fill(idTable);

                ulong id = (ulong)(idTable.Rows.Count + 1);

                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();
                MySqlCommand createUserCommand = new($"INSERT INTO `Users`(`UserId`, `Login`, `Password`, `Name`, `LastName`, `SurName`, `Course`, `Specialization`, `University`) VALUES ({id},'{login}','{pass}','{name}','{lastname}','{surname}',{course},'{specialization}','{univer}')", Connection);
                if (createUserCommand.ExecuteNonQuery() > 0)
                {
                    output = $"Аккаунт: `{login}` успешно создан";
                }
                else
                {
                    output = "Ошибка";
                }
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();

                output = $"Аккаунт: `{login}` успешно создан";
                return new(id, login, pass, name, lastname, surname, course, specialization, univer, null, null, null);
            }

            output = "Аккаунт с таким логином уже существует";
            return null;
        }

        public static User? GetUserById(ulong userId)
        {
            MySqlCommand getUserCommand = new($"SELECT * FROM `Users` WHERE `UserId`={userId}", Connection);
            MySqlDataAdapter getUser = new(getUserCommand);
            DataTable user = new();
            getUser.Fill(user);

            if (user.Rows.Count > 0)
            {
                string login = (string)user.Rows[0][1];
                string pass = (string)user.Rows[0][2];
                string name = (string)user.Rows[0][3];
                string lastname = (string)user.Rows[0][4];
                string surname = (string)user.Rows[0][5];
                int course = (int)user.Rows[0][6];
                string specialization = (string)user.Rows[0][8];
                string university = (string)user.Rows[0][8];
                List<Tag> tags = GetTags(userId, false);
                List<Discussion> activeDiscussions = GetDiscussionsByApplicantId(userId, false);
                List<Discussion> closedDiscussions = GetDiscussionsByApplicantId(userId, true);

                return new(userId, login, pass, name, lastname, surname, course, specialization, university, tags, activeDiscussions, closedDiscussions);
            }

            return null;
        }

        public static string? GetUserNameById(ulong userId)
        {
            MySqlCommand getUserNameCommand = new($"SELECT * FROM `Users` WHERE `UserId`={userId}", Connection);
            MySqlDataAdapter getUserName = new(getUserNameCommand);
            DataTable userName = new();
            getUserName.Fill(userName);
            return userName.Rows[0][3].ToString();
        }

        public static List<Tag>? GetTags(ulong id, bool isDiscussion)
        {
            DataTable tagsId = new();

            if (isDiscussion)
            {
                MySqlCommand getTagsIdCommand = new($"SELECT * FROM `Tags-Discussions` WHERE `DiscussionId`={id}", Connection);
                MySqlDataAdapter getTagsId = new(getTagsIdCommand);
                getTagsId.Fill(tagsId);
            }
            else
            {
                MySqlCommand getTagsIdCommand = new($"SELECT * FROM `Tags-Users` WHERE `UserId`={id}", Connection);
                MySqlDataAdapter getTagsId = new(getTagsIdCommand);
                getTagsId.Fill(tagsId);
            }

            List<Tag> tagList = new();

            for (int i = 0; i < tagsId.Rows.Count; i++)
            {
                MySqlCommand getTagsCommand = new($"SELECT * FROM `Tags` WHERE `Id`={(ulong)tagsId.Rows[i][0]}", Connection);
                MySqlDataAdapter getTags = new(getTagsCommand);
                DataTable tags = new();
                getTags.Fill(tags);
                tagList.Add(new((ulong)tags.Rows[0][0], (string)tagsId.Rows[0][1], (string)tagsId.Rows[0][2]));
            }

            return tagList;
        }

        public static Discussion? CreateDiscussion(ulong applicantId, string topic, string text)
        {
            MySqlCommand getDiscussionCommand = new($"SELECT * FROM `Discussions` WHERE `Topic`='{topic}'", Connection);
            MySqlDataAdapter getDisscusion = new(getDiscussionCommand);
            DataTable discussion = new();
            getDisscusion.Fill(discussion);

            if (discussion.Rows.Count > 0)
            {
                return null;
            }
            else
            {
                getDiscussionCommand = new($"SELECT * FROM `Discussions`", Connection);
                getDisscusion = new(getDiscussionCommand);
                discussion = new();
                getDisscusion.Fill(discussion);

                ulong discussionId = (ulong)discussion.Rows.Count + 1;
                MySqlCommand createDiscussion = new($"INSERT INTO `Discussions`(`Discussionid`,`ApplicatorId`,`Topic`,`Text`,`IsClosed`) VALUES ({discussionId},{applicantId},'{topic}','{text}',0)", Connection);

                if (Connection.State == ConnectionState.Closed) Connection.Open();
                if (createDiscussion.ExecuteNonQuery() > 0) { }
                if (Connection.State == ConnectionState.Open) Connection.Close();

                return GetDiscussionById(discussionId);
            }
        }

        public static Discussion? GetDiscussionById(ulong discussionId)
        {
            MySqlCommand getDiscussionCommand = new($"SELECT * FROM `Discussions` WHERE `DiscussionId`={discussionId}", Connection);
            MySqlDataAdapter getDisscusion = new(getDiscussionCommand);
            DataTable discussion = new();
            getDisscusion.Fill(discussion);

            if (discussion.Rows.Count > 0)
            {
                ulong applicantId = (ulong)discussion.Rows[0][1];
                string topic = (string)discussion.Rows[0][2];
                string text = (string)discussion.Rows[0][3];
                bool isClosed = (bool)discussion.Rows[0][4];
                List<Tag> tags = GetTags(discussionId, true);
                List<Answer> answers = GetAnswersByDiscussionId(discussionId);

                return new(discussionId, applicantId, topic, text, isClosed, tags, answers);
            }

            return null;
        }

        public static List<Discussion> GetlastDescutions()
        {
            MySqlCommand getDiscussionsCommand = new($"SELECT * FROM `Discussions`", Connection);
            MySqlDataAdapter getDisscusions = new(getDiscussionsCommand);
            DataTable discussions = new();
            getDisscusions.Fill(discussions);

            List<Discussion> discussionList = new();

            if (discussions.Rows.Count > 0)
            {
                int count = discussions.Rows.Count < 5 ? 0 : discussions.Rows.Count - 5;
                for (int i = discussions.Rows.Count - 1; i > count; i--)
                {
                    ulong discussionId = (ulong)discussions.Rows[i][0];
                    ulong applicantId = (ulong)discussions.Rows[i][1];
                    string topic = (string)discussions.Rows[i][2];
                    string text = (string)discussions.Rows[i][3];
                    bool isClosed = (bool)discussions.Rows[i][4];
                    List<Tag> tags = GetTags(discussionId, true);
                    List<Answer> answers = GetAnswersByDiscussionId(discussionId);
                    discussionList.Add(new(discussionId, applicantId, topic, text, isClosed, tags, answers));
                }

                return discussionList;
            }

            return null;
        }

        public static List<Discussion> GetDiscussionsByApplicantId(ulong applicantId, bool closed)
        {
            MySqlCommand getDiscussionsCommand = new($"SELECT * FROM `Discussions` WHERE `ApplicatorId`={applicantId} AND `IsClosed`={closed}", Connection);
            MySqlDataAdapter getDisscusions = new(getDiscussionsCommand);
            DataTable discussions = new();
            getDisscusions.Fill(discussions);

            List<Discussion> discussionList = new();

            for (int i = 0; i < discussions.Rows.Count; i++)
            {
                ulong id = (ulong)discussions.Rows[0][0];
                string topic = (string)discussions.Rows[0][2];
                string text = (string)discussions.Rows[0][3];
                bool isClosed = closed;
                List<Tag> tags = GetTags(id, true);
                List<Answer> answers = new();
                discussionList.Add(new(id, applicantId, topic, text, isClosed, tags, answers));
            }

            return discussionList;
        }

        public static List<Discussion> GetDiscussionsByTagId(ulong tagId, bool isClosed)
        {
            MySqlCommand getDiscussionsCommand = new($"SELECT * FROM `Tags-Discussions` WHERE `TagId`={tagId} AND `IsClosed`={isClosed}", Connection);
            MySqlDataAdapter getDisscusions = new(getDiscussionsCommand);
            DataTable discussions = new();
            getDisscusions.Fill(discussions);

            List<Discussion> discussionList = new();

            for (int i = 0; i < discussions.Rows.Count; i++)
            {
                ulong id = (ulong)discussions.Rows[0][0];
                ulong applicantId = (ulong)discussions.Rows[0][1];
                string topic = (string)discussions.Rows[0][2];
                string text = (string)discussions.Rows[0][3];
                List<Tag> tags = GetTags(id, true);
                List<Answer> answers = new();
                discussionList.Add(new(id, applicantId, topic, text, isClosed, tags, answers));
            }

            return discussionList;
        }

        public static Answer? AddAnswer(ulong discussionId, ulong userId, string answerText)
        {
            string command = $"INSERT INTO `Answers`(`DiscussionId`, `UserId`, `Text`, `LikesCount`, `DiskikesCount`, `Correct`) VALUES ({discussionId},{userId},'{answerText}',0,0,0)";
            MySqlCommand addAnswerCommand = new(command, Connection);

            if (Connection.State == ConnectionState.Closed) Connection.Open();
            if (addAnswerCommand.ExecuteNonQuery() > 0) { }
            if (Connection.State == ConnectionState.Open) Connection.Close();

            return new(discussionId, userId, answerText, 0, 0, false);
        }

        public static List<Answer> GetAnswersByUserId(ulong userId)
        {
            MySqlCommand getAnswersCommand = new($"SELECT * FROM `Answers-Discussions` WHERE `UserId`={userId}", Connection);
            MySqlDataAdapter getAnswers = new(getAnswersCommand);
            DataTable answers = new();
            getAnswers.Fill(answers);

            List<Answer> answerList = new();

            for (int i = 0; i < answers.Rows.Count; i++)
            {
                ulong discussionId = (ulong)answers.Rows[0][0];
                string text = (string)answers.Rows[0][2];
                int likesCount = (int)answers.Rows[0][3];
                int dislikesCount = (int)answers.Rows[0][4];
                bool correct = (bool)answers.Rows[0][5];
                answerList.Add(new(discussionId, userId, text, likesCount, dislikesCount, correct));
            }

            return answerList;
        }

        public static List<Answer> GetAnswersByDiscussionId(ulong discussionId)
        {
            MySqlCommand getAnswersCommand = new($"SELECT * FROM `Answers` WHERE `DiscussionId`={discussionId}", Connection);
            MySqlDataAdapter getAnswers = new(getAnswersCommand);
            DataTable answers = new();
            getAnswers.Fill(answers);

            List<Answer> answerList = new();

            for (int i = 0; i < answers.Rows.Count; i++)
            {
                ulong userId = (ulong)answers.Rows[i][1];
                string text = (string)answers.Rows[i][2];
                int likesCount = (int)answers.Rows[i][3];
                int dislikesCount = (int)answers.Rows[i][4];
                bool correct = (bool)answers.Rows[i][5];
                answerList.Add(new(discussionId, userId, text, likesCount, dislikesCount, correct));
            }

            return answerList;
        }
    }
}
