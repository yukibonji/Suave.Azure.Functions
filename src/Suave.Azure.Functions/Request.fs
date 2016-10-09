﻿module Suave.Azure.Functions.Request

open Suave.Http
open System.Net.Http

let suaveHttpMethod (httpMethod : HttpMethod) =
   match httpMethod.Method with
   | "Get" -> HttpMethod.GET
   | "Put" -> HttpMethod.PUT
   | "Post" -> HttpMethod.POST
   | "Delete" -> HttpMethod.DELETE
   | "Patch" -> HttpMethod.PATCH
   | "Head" -> HttpMethod.HEAD
   | "Trace" -> HttpMethod.TRACE
   | "Connect" -> HttpMethod.CONNECT
   | "OPTIONS" -> HttpMethod.OPTIONS
   | x -> HttpMethod.OTHER x

let httpMethod (suaveHttpMethod : Suave.Http.HttpMethod) =  
  match suaveHttpMethod with
  | OTHER _ -> 
    None
  | _ -> 
    let method : System.String = suaveHttpMethod |> string
    let normalized = method.ToLowerInvariant()
    string(System.Char.ToUpper(normalized.[0])) + normalized.Substring(1)
    |> HttpMethod |> Some

let suaveHttpRequestHeaders (httpRequestHeaders : Headers.HttpRequestHeaders) = 
  httpRequestHeaders
  |> Seq.collect (fun h -> h.Value |> Seq.map (fun v -> (h.Key, v)))
  |> Seq.toList

let httpRequestHeaders (suaveHttpRequestHeaders : (string * string) list) =
  let req = new HttpRequestMessage()
  suaveHttpRequestHeaders |> List.iter req.Headers.Add
  req.Headers

let suaveRawForm (content : HttpContent) = async {
  return! content.ReadAsByteArrayAsync() |> Async.AwaitTask
}

let httpContent (rawForm : byte[]) =
  new ByteArrayContent(rawForm) :> HttpContent

let suaveRawQuery (requestUri : System.Uri) =
  if requestUri.Query.Length > 1 then
    requestUri.Query.Substring(1)
  else
    ""

let suaveHttpRequest (httpRequestMessage : HttpRequestMessage) =
  {
    HttpRequest.empty with 
      headers = suaveHttpRequestHeaders httpRequestMessage.Headers
      url = httpRequestMessage.RequestUri
  }
  |> async.Return
  