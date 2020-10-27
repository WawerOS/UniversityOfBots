/*
This file *is NOT* licensed under the MPL 2.0.
This is a derivative of the file "TimeConverters.cs" of the DSharpPlus project. 
As such, this file is licensed under the MIT License. Full text below.

The MIT License (MIT)

Copyright (c) 2015 Mike Santiago
Copyright (c) 2016-2018 DSharpPlus Development Team

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Globalization;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace Gauss.Converters
{
    public class DateTimeConverter : IArgumentConverter<DateTime>
    {
        Task<Optional<DateTime>> IArgumentConverter<DateTime>.ConvertAsync(string value, CommandContext context)
        {
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var result))
                return Task.FromResult(new Optional<DateTime>(result));

            return Task.FromResult(Optional.FromNoValue<DateTime>());
        }
    }
}