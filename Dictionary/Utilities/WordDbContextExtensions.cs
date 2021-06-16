using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dictionary.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dictionary.Utilities
{
    public static class WordDbContextExtensions
    {
        public static OperationResult GetWord(this WordDbContext context, int id)
        {
            if (context.Words.Any(w => w.Id == id))
            {
                Word w = context.Words.Find(id);
                return new OperationResult { Oks = new List<Word> { w } };
            }
            else
            {
                return new OperationResult { NotFounds = new List<string> { $"Request word with Id {id} not found." } };
            }
        }

        public static async Task<OperationResult> AddWord(this WordDbContext context, string word, ILogger logger)
        {
            OperationResult or = new OperationResult();
            Word w = new Word(word);
            try
            {
                context.Words.Add(w);
                await context.SaveChangesAsync();
                or.Oks.Add(w);
            }
            catch (Exception e)
            {
                List<Word> wds = context.Words.Where(w => w.WordW.ToLower() == word.ToLower()).ToList();
                if (wds.Any())
                {
                    logger.LogError(e, "failed to add/{0}, word already in db with Id {1}", word, wds.First().Id);
                }
                else
                {
                    logger.LogError(e, "failed to add/{0}", word);
                }
                or.Conflicts.AddRange(wds);
            }
            return or;
        }

        public static async Task<OperationResult> AddWordBatch(this WordDbContext context, BatchWords words, ILogger logger)
        {
            bool changed = false;
            OperationResult or = new OperationResult();
            OperationResult orCheck = new OperationResult();
            List<string> checkedWords = new List<string>();
            int checkedCount = 0;
            int MaxCheckCount = 10;
            foreach (var word in words.Words)
            {
                checkedCount++;
                checkedWords.Add(word);
                Word w = new Word(word);
                try
                {
                    context.Words.Add(w);
                    changed = true;
                    orCheck.Oks.Add(w);
                }
                catch (Exception e)
                {
                    List<Word> wds = context.Words.Where(w => w.WordW.ToLower() == word.ToLower()).ToList();
                    if (wds.Any())
                    {
                        logger.LogError(e, "failed to add/{0}, word already in db with Id {1}", word, wds.First().Id);
                    }
                    else
                    {
                        logger.LogError(e, "failed to add/{0}", word);
                    }
                    orCheck.Conflicts.AddRange(wds);
                }

                if (checkedCount == MaxCheckCount)
                {
                    if (changed)
                    {
                        await context.AddCheckedWords(checkedWords, orCheck, or, logger);
                    }
                    checkedWords.Clear();
                    checkedCount = 0;
                }
            }

            if (checkedCount > 0 && changed)
            {
                await context.AddCheckedWords(checkedWords, orCheck, or, logger);
            }
            return or;
        }

        private static async Task AddCheckedWords(this WordDbContext context, List<string> checkedWords,
            OperationResult orCheck, OperationResult or, ILogger logger)
        {
            try
            {
                await context.SaveChangesAsync();
                or.Append(orCheck);
            }
            catch (Exception e1)
            {
                logger.LogError(e1, "failed to batch add words '{0}', one or more words already in db.",
                    JsonConvert.SerializeObject(checkedWords));
                foreach (var wd in checkedWords)
                {
                    or.Append(await context.AddWord(wd, logger));
                }
            }
        }

        public static async Task<OperationResult> DeleteWord(this WordDbContext context, string word)
        {
            if (context.Words.Any(w => w.WordW.ToLower() == word.ToLower()))
            {
                List<Word> wds = context.Words.Where(w => w.WordW.ToLower() == word.ToLower()).ToList();
                context.Words.RemoveRange(wds);
                await context.SaveChangesAsync();
                return new OperationResult { Oks = wds };
            }
            else
            {
                return new OperationResult { NotFounds = new List<string> { word } };
            }
        }
    }
}
