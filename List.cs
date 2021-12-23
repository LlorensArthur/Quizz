namespace Quizz
{
    // List with a double container used to create the logs of the user
    public class List<T1, T2>
    {
        public List<T1> question = new List<T1>();
        public List<T2> userAnswer = new List<T2>();
        public void Add(T1 t1, T2 t2)
        {
            question.Add(t1);
            userAnswer.Add(t2);
        }
    }
}