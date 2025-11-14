using System;
using System.Collections.Generic;

[Serializable]
public class ChatCompletionRequest
{
    public string model;
    public List<Message> messages;
}

[Serializable]
public class Message
{
    public string role;
    public string content;
}

[Serializable]
public class ChatCompletionResponse
{
    public List<Choice> choices;
}

[Serializable]
public class Choice
{
    public Message message;
}

[System.Serializable]
public class QuestionList
{
    public string[] questions;
}