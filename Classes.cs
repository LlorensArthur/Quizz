namespace Quizz
{
    // This class contains the question sentence and a list of all of the possible answers.
    // For the logic of the current architecture, there should be only one valid answer in a question.
    public class Question
    {
        public string question = "Question not defined";
        public List<Answer> answers = new List<Answer>();

        public Question(string _question, List<Answer> _answers)
        {
            question = _question;
            answers = _answers;
        }
        // Used when we want to know was was the right answer in the logs
        public Answer getRightAnswer()
        {
            foreach (var answer in answers)
            {
                if(answer.isTrue)
                    return answer;
            }
            return null;
        }
    }
    // An answer to a question.
    // An answer is contained in a list that is contained in a Question object.
    public class Answer
    {
        public string answer = "Answer not defined";
        public bool isTrue;
        public Answer(string _answer, bool _isTrue)
        {
            answer = _answer;
            isTrue = _isTrue;
        }
    }
}
