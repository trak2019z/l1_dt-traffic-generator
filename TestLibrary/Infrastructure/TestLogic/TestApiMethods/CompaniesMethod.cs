﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using testdll.TestApiMethods;
using TestLibrary.Infrastructure.TestLogic.API.Request.BuyOffers;
using TestLibrary.Infrastructure.TestLogic.API.Request.Company;
using TestLibrary.Infrastructure.TestLogic.API.Response.BuyOffers;
using TestLibrary.Infrastructure.TestLogic.API.Response.Companies;

namespace TestLibrary.Infrastructure.TestLogic.TestApiMethods
{
    class BuyOfferMethods
    {
        public static ReturnGetBuyOffers GetUserBuyOffers(int paramId, int userId, string token)
        {
            ReturnGetBuyOffers returnedBuyOffers = new ReturnGetBuyOffers();
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                string resp = "";
                using (var client = new WebClient())
                {
                    client.Headers.Add("Content-Type:application/json"); //Content-Type  
                    client.Headers.Add("Accept:application/json");
                    client.Headers.Add("Authorization", "Bearer " + token);
                    var result = client.DownloadString(GET_URLs.BuyOffers); //URI  
                    resp = result;
                    GetBuyOffersByUserIdResponseModel response = new GetBuyOffersByUserIdResponseModel();
                    response = JsonConvert.DeserializeObject<GetBuyOffersByUserIdResponseModel>(resp);
                    returnedBuyOffers.tests = new List<Test>();
                    returnedBuyOffers.buy = new List<BuyOfferModel>();
                    returnedBuyOffers.buy.AddRange(response.buyOffers);
                    watch.Stop();
                    long TestTime = watch.ElapsedMilliseconds;
                    if (response.execDetails.ExecTime == null || response.execDetails.DbTime == null ||
                        TestTime == null)
                    {
                        response.execDetails.ExecTime = 0;
                        response.execDetails.DbTime = 0;
                        TestTime = 0;
                    }

                    returnedBuyOffers.tests.Add(new Test(DateTime.Now, paramId, userId, (int)EndpointEnum.GetBuyOffers,
                        response.execDetails.DbTime.Value, TestTime, response.execDetails.ExecTime.Value));
                    Program.testsLis.AddRange(returnedBuyOffers.tests);
                    Program.user.Where(u => u.userId == userId).ToList()
                        .ForEach(ug => ug.userBuyOffer = returnedBuyOffers.buy);

                }
            }
            catch (Exception e)
            {
                returnedBuyOffers.tests = new List<Test>();
                returnedBuyOffers.tests.Add(new Test(DateTime.Now, paramId, userId, (int)EndpointEnum.GetBuyOffers,
                    0, 0, 0));
                Program.testsLis.AddRange(returnedBuyOffers.tests);


            }

            return returnedBuyOffers;
        }

        public static ReturnAddBuyOffers AddBuyOffer(int testParam, string token, int USERID, double minBuyPrice, double maxBuyPrice)
        {
            ReturnAddBuyOffers ret = new ReturnAddBuyOffers();
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();


                Random random = new Random();
                int amount = random.Next(1, 50);
                double price = random.Next((int)minBuyPrice, (int)maxBuyPrice);
                List<CompanyModel> companies = new List<CompanyModel>();
                List<int> companiesId = new List<int>();

                companies = Program.comp;
                foreach (var x in companies)
                {
                    companiesId.Add(x.Id);
                }


                int companyId = companiesId[random.Next(0, companiesId.Count)];
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(POST_URLs.AddBuyOffers);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.Method = "POST";

                CreateBuyOfferRequest BuyOfferRequest = new CreateBuyOfferRequest();
                BuyOfferRequest.companyId = companyId;
                BuyOfferRequest.amount = amount;
                BuyOfferRequest.price = price;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string output = JsonConvert.SerializeObject(BuyOfferRequest);
                    streamWriter.Write(output);
                }

                string resp = "";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    resp = result;
                }
                CreateBuyOfferResponseModel response = new CreateBuyOfferResponseModel();
                response = JsonConvert.DeserializeObject<CreateBuyOfferResponseModel>(resp);
                watch.Stop();
                long TestTime = watch.ElapsedMilliseconds;
                ret.tests = new List<Test>();
                if (response.execDetails.ExecTime == null || response.execDetails.DbTime == null || TestTime == null)
                {
                    response.execDetails.ExecTime = 0;
                    response.execDetails.DbTime = 0;
                    TestTime = 0;
                }
                ret.tests.Add(new Test(DateTime.Now, testParam, USERID, (int)EndpointEnum.AddUser, response.execDetails.DbTime.Value, TestTime, response.execDetails.ExecTime.Value));
                Program.testsLis.AddRange(ret.tests);
            }
            catch (Exception e)
            {
                ret.tests = new List<Test>();
                ret.tests.Add(new Test(DateTime.Now, testParam, USERID, (int)EndpointEnum.AddBuyOffer, 0, 0, 0));
                Program.testsLis.AddRange(ret.tests);
            }

            return ret;

        }

        public static ReturnPUTBuyOffers PutBuyOffers(int testParam, string token, int USERID)
        {
            ReturnPUTBuyOffers ret = new ReturnPUTBuyOffers();
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();


                Random random = new Random();


                List<BuyOfferModel> userBuyOffers = new List<BuyOfferModel>();
                List<int> IdUserBuyOffers = new List<int>();


                foreach (var x in Program.user)
                {
                    if (x.userToken == token)
                    {
                        userBuyOffers = x.userBuyOffer;
                        foreach (var y in userBuyOffers)
                        {
                            if (y.IsValid == true)
                            {
                                IdUserBuyOffers.Add(y.Id);
                            }

                        }
                    }
                }

                int buyOfferId = IdUserBuyOffers[random.Next(0, IdUserBuyOffers.Count)];
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(PUT_URLs.WithdrawBuyOffers + buyOfferId.ToString());
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.Method = "PUT";

                CreateBuyOfferRequest buyOfferRequest = new CreateBuyOfferRequest();

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    //string output = JsonConvert.SerializeObject(sellOfferRequest);
                    //streamWriter.Write(output);
                }


                string resp = "";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    resp = result;
                }
                WithdrawBuyOfferResponseModel response = new WithdrawBuyOfferResponseModel();
                response = JsonConvert.DeserializeObject<WithdrawBuyOfferResponseModel>(resp);
                watch.Stop();
                long TestTime = watch.ElapsedMilliseconds;
                ret.tests = new List<Test>();
                if (response.execDetails.ExecTime == null || response.execDetails.DbTime == null || TestTime == null)
                {
                    response.execDetails.ExecTime = 0;
                    response.execDetails.DbTime = 0;
                    TestTime = 0;
                }
                ret.tests.Add(new Test(DateTime.Now, testParam, USERID, (int)EndpointEnum.PUTBuyOffer, response.execDetails.DbTime.Value, TestTime, response.execDetails.ExecTime.Value));
                Program.testsLis.AddRange(ret.tests);
            }
            catch (Exception e)
            {
                ret.tests = new List<Test>();
                ret.tests.Add(new Test(DateTime.Now, testParam, USERID, (int)EndpointEnum.PUTBuyOffer, 0, 0, 0));
                Program.testsLis.AddRange(ret.tests);
            }

            return ret;

        }

    }
}
