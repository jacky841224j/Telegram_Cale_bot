using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;

namespace ConsoleApp1
{
    struct BotUpdate
    {
        public string text;
        public long id;
        public string username;
    }
    class Program
    {
        public static TelegramBotClient Client;
        static string fileName = "updates.json";
        static List<BotUpdate> botupdates = new List<BotUpdate>();
        private static ChatAction chataction;

        static void Main(string[] args)
        {
            try
            {
                var botUpdateString = System.IO.File.ReadAllText(fileName);
                botupdates = JsonConvert.DeserializeObject<List<BotUpdate>>(botUpdateString) ?? botupdates;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Reading or DeserializeObject" + ex);
            }

            Client = new TelegramBotClient("1888080710:AAFX4-h2MCh8r3VpTVztDoamVNo_cSiJGus");
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[]
                {
                    UpdateType.Message,
                    UpdateType.EditedMessage,
                }
            };
            Client.StartReceiving(updateHandler, pollingErrorHandler, receiverOptions);
            //var t = await Client.SendTextMessage(806077724, "text message");
            //https://api.telegram.org/bot1546506272:AAFeRnuJOsjfxYIo_LcsSOMRgIfl24v5fzY/sendMessage?chat_id=-1001353169029&text=%E6%A9%9F%E5%99%A8%E4%BA%BA%E5%9B%9E%E8%A9%B1%E6%B8%AC%E8%A9%A6
            //Client.SendChatActionAsync(-1001353169029, ChatAction.Typing);
            Console.ReadLine();
        }

        private static Task pollingErrorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
        static bool cale = false;
        static float TA = 0;
        static float Stop_Loss = 0;
        private static async Task updateHandler(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Type != UpdateType.Message)
                return;
            // Only process text messages
            if (update.Message!.Type != MessageType.Text)
                return;

            if (update.Type == UpdateType.Message)
            {
                if (update.Message.Type == MessageType.Text)
                {
                    var _botUpdate = new BotUpdate
                    {
                        text = update.Message.Text,
                        id = update.Message.Chat.Id,
                        username = update.Message.Chat.Username
                    };
                    botupdates.Add(_botUpdate);
                    var botUpdatesString = JsonConvert.SerializeObject(botupdates);
                    System.IO.File.WriteAllText(fileName, botUpdatesString);

                    //var sentMessage = await Client.SendTextMessageAsync(
                    //   chatId: update.Message.Chat.Id,
                    //   text: "Hello～" + update.Message.From.LastName + "，你的ID是： " + update.Message.Chat.Id + "，你傳了：" + update.Message.Text + " 給我",
                    //   cancellationToken: cancellationToken);

                    if (cale && TA > 0)
                    {
                        bool isParsable = float.TryParse(messageText, out Stop_Loss);
                        if (isParsable)
                        {
                            Stop_Loss = Stop_Loss  / 100 ;
                            Message message = await Client.SendTextMessageAsync(
                            chatId: chatId,
                            disableNotification: true,
                            replyToMessageId: update.Message.MessageId,
                            text: "停損金額為："+ (TA - TA  * Stop_Loss),
                            cancellationToken: cancellationToken);
                            cale = false;
                            TA = 0;
                            Stop_Loss = 0;
                        }
                    }

                    if (cale && TA == 0 && messageText != "/cale")
                    {
                        bool isParsable = float.TryParse(messageText, out TA);
                        if (!isParsable && TA > 0)
                        {
                            Message message = await Client.SendTextMessageAsync(
                            chatId: chatId,
                            disableNotification: true,
                            replyToMessageId: update.Message.MessageId,
                            text: "錯誤：請輸入數字",
                            cancellationToken: cancellationToken);
                        }
                        else
                        {
                            Message message = await Client.SendTextMessageAsync(
                            chatId: chatId,
                            disableNotification: true,
                            replyToMessageId: update.Message.MessageId,
                            text: "請輸入停損比率(百分比):",
                            cancellationToken: cancellationToken);
                        }
                    }

                   

                    if (messageText == "exit")
                    {
                        cale = false;
                        TA = 0;
                        Stop_Loss = 0;
                        Message message = await Client.SendTextMessageAsync(
                        chatId: chatId,
                        disableNotification: true,
                        replyToMessageId: update.Message.MessageId,
                        text: "已離開程式",
                        cancellationToken: cancellationToken);
                    }


                    if (messageText == "/cale")
                    {
                        Message message = await Client.SendTextMessageAsync(
                        chatId: chatId,
                        disableNotification: true,
                        replyToMessageId: update.Message.MessageId,
                        text: "請輸入總資金：",
                        cancellationToken: cancellationToken);
                        cale = true;
                    }

                    
                    if (messageText == "meme")
                    {
                        var sentMessage = await Client.SendPhotoAsync(
                        chatId: chatId,
                        photo: "https://i.redd.it/uhkj4abc96r61.jpg",
                        caption: "<b>MEME</b>",
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken);
                    }

                    if (messageText == "sound")
                    {
                        Message message = await Client.SendAudioAsync(
                         chatId: chatId,
                         audio: "https://github.com/TelegramBots/book/raw/master/src/docs/audio-guitar.mp3",
                         cancellationToken: cancellationToken);
                    }

                    //if message is "countdown" .. bot answer with a countdown video.
                    if (messageText == "countdown")
                    {
                        Message message = await Client.SendVideoAsync(
                        chatId: chatId,
                        video: "https://raw.githubusercontent.com/TelegramBots/book/master/src/docs/video-countdown.mp4",
                        thumb: "https://raw.githubusercontent.com/TelegramBots/book/master/src/2/docs/thumb-clock.jpg",
                        supportsStreaming: true,
                        cancellationToken: cancellationToken);
                    }

                    //if message is "album" .. bot answer with multiple images.
                    if (messageText == "album")
                    {
                        Message[] messages = await Client.SendMediaGroupAsync(
                        chatId: chatId,
                        media: new IAlbumInputMedia[]
                        {
                new InputMediaPhoto("https://cdn.pixabay.com/photo/2017/06/20/19/22/fuchs-2424369_640.jpg"),
                new InputMediaPhoto("https://cdn.pixabay.com/photo/2017/04/11/21/34/giraffe-2222908_640.jpg"),
                        },
                        cancellationToken: cancellationToken);
                    }

                    //if message is "doc" .. bot answer with a doc.
                    if (messageText == "doc")
                    {
                        //Use sendDocument method to send general files.
                        Message message = await Client.SendDocumentAsync(
                        chatId: chatId,
                        document: "https://github.com/TelegramBots/book/raw/master/src/docs/photo-ara.jpg",
                        caption: "<b>Ara bird</b>. <i>Source</i>: <a href=\"https://pixabay.com\">Pixabay</a>",
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken);
                    }

                    //if message is "album" .. bot answer with multiple images.
                    if (messageText == "泱泱")
                    {
                        //Use sendAnimation method to send animation files(GIF or H.264 / MPEG - 4 AVC video without sound).
                        Message message = await Client.SendAnimationAsync(
                        chatId: chatId,
                        animation: "https://i.imgur.com/Hq9T1OB.gif",
                        //caption: "Waves",
                        cancellationToken: cancellationToken);
                    }
                    if (messageText == "林襄")
                    {
                        //Use sendAnimation method to send animation files(GIF or H.264 / MPEG - 4 AVC video without sound).
                        Message message = await Client.SendAnimationAsync(
                        chatId: chatId,
                        animation: "https://media.tenor.com/5tF9tO4G7TQAAAAd/winnieeelin-%E6%9E%97%E8%A5%84.gif",
                        //caption: "Waves",
                        cancellationToken: cancellationToken);
                    }
                }
            }
        }
    }
}
