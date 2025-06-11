#!/bin/bash
set -e 

cd ProductsApi
dotnet tool restore
dotnet ef database update
dotnet build