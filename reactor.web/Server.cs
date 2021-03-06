﻿/*--------------------------------------------------------------------------

Reactor.Web

The MIT License (MIT)

Copyright (c) 2015 Haydn Paterson (sinclair) <haydn.developer@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

---------------------------------------------------------------------------*/

using Reactor.Http;
using System;

namespace Reactor.Web
{
    public class Server
    {
        private Reactor.Http.Server                      httpserver;

        private Reactor.Web.Router                       router;

        private Reactor.Action<Reactor.Http.HttpContext> servercb;

        public event Action<Exception>                   OnError;

        public Server(Reactor.Http.Server httpserver)
        {
            this.router               = new Router();

            this.httpserver           = httpserver;

            this.servercb             = this.httpserver.OnContext;

            this.httpserver.OnContext = this.OnHttpContext;

            this.httpserver.OnError += (error) =>
            {
                if (this.OnError != null) {

                    this.OnError(error);
                }
            };
        }

        public Server(): this(Reactor.Http.Server.Create())
        {

        }

        private void OnHttpContext(HttpContext context)
        {
            this.router.Handler(new Web.Context(context), () =>
            {
                if(this.servercb != null)
                {
                    this.servercb(context);

                    return;
                }

                context.Response.StatusCode  = 404;

                context.Response.ContentType = "text/plain";

                context.Response.Write(Reactor.Buffer.Create(System.Text.Encoding.UTF8.GetBytes(context.Request.Url.AbsolutePath + " not found")));

                context.Response.End();
            });
        }

        #region Methods

        public Router Use(Reactor.Web.Middleware middleware)
        {
            return this.router.Use(middleware);
        }

        public Router Get(string pattern, Reactor.Action<Context> handler)
        {
            return this.router.Get(pattern, handler);
        }

        public Router Post(string pattern, Reactor.Action<Context> handler)
        {
            return this.router.Post(pattern, handler);
        }

        public Router Put(string pattern, Reactor.Action<Context> handler)
        {
            return this.router.Put(pattern, handler);
        }

        public Router Delete(string pattern, Reactor.Action<Context> handler)
        {
            return this.router.Delete(pattern, handler);
        }

        public Router Options(string pattern, Reactor.Action<Context> handler)
        {
            return this.router.Options(pattern, handler);
        }

        public Router Get(string pattern, Reactor.Web.Middleware[] middleware, Reactor.Action<Context> handler)
        {
            return this.router.Get(pattern, middleware, handler);
        }

        public Router Post(string pattern, Reactor.Web.Middleware[] middleware, Reactor.Action<Context> handler)
        {
            return this.router.Post(pattern, middleware, handler);
        }

        public Router Put(string pattern, Reactor.Web.Middleware[] middleware, Reactor.Action<Context> handler)
        {
            return this.router.Put(pattern, middleware, handler);
        }

        public Router Delete(string pattern, Reactor.Web.Middleware[] middleware, Reactor.Action<Context> handler)
        {
            return this.router.Delete(pattern, middleware, handler);
        }

        public Router Options(string pattern, Reactor.Web.Middleware[] middleware, Reactor.Action<Context> handler)
        {
            return this.router.Options(pattern, middleware, handler);
        }

        #endregion

        public Server Listen(int Port, Action<Exception> callback)
        {
            this.httpserver.Listen(Port, callback);

            return this;
        }

        public Server Listen(int Port)
        {
            this.httpserver.Listen(Port);

            return this;
        }

        public Server Stop()
        {
            this.httpserver.Stop();

            return this;
        }

        #region Statics

        public static Server Create(Reactor.Http.Server httpserver)
        {
            return new Server(httpserver);
        }

        public static Server Create()
        {
            return new Server();
        }

        #endregion
    }
}