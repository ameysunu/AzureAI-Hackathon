using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using OpenAI.Images;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacifyFunctions.Helpers
{
    public class OpenAIHelper
    {
        public string apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
        public string openAiEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        public AzureOpenAIClient azureClient;
        public ChatClient chatClient;
        public ImageClient imageClient;
        public ILogger logger;

        public OpenAIHelper(ILogger logger)
        {
            this.logger = logger;
            InitOpenAI().GetAwaiter().GetResult();
        }

        public async Task InitOpenAI()
        {

            azureClient = new(
                new Uri(openAiEndpoint),
                new ApiKeyCredential(apiKey));

            chatClient = azureClient.GetChatClient("gpt-4o-mini");
            imageClient = azureClient.GetImageClient("dall-e-3");

            logger.LogInformation("Azure Open AI Initiated");

        }

        public async Task GenerateImagePrompt(String imagePrompt)
        {
            var imageGenOptions = new ImageGenerationOptions()
            {
                Size = GeneratedImageSize.W1792xH1024
            };

            var imageGenerator = await imageClient.GenerateImageAsync(imagePrompt, imageGenOptions);
            logger.LogInformation(imageGenerator.Value.ImageUri.ToString());
        }

        public async Task<String> SendTextMessagePrompt(String systemText, String userText)
        {
            var systemMessage = ChatMessage.CreateSystemMessage(systemText);
            var userMessage = ChatMessage.CreateUserMessage(userText);

            ChatMessage[] chatMessages = new ChatMessage[]
            {
                systemMessage,
                userMessage
            };

            var response = await chatClient.CompleteChatAsync(chatMessages);

            if (response != null && response.Value != null && response.Value.Content.Count > 0)
            {
                var responseData = response.Value.Content.FirstOrDefault();

                if (responseData.Text != null)
                {
                    logger.LogInformation(responseData.Text);
                    return(responseData.Text);
                } 
                else
                {
                    logger.LogDebug("Response null from OpenAI");
                    return null;
                }
            }

            return null;
        }
    }
}
