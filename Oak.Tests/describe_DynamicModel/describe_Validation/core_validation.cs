﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSpec;
using Oak.Tests.describe_Validation.Classes;

namespace Oak.Tests.describe_DynamicModel.describe_Validation
{
    class core_validation : nspec
    {
        dynamic book;

        string firstError;

        bool isValid;

        dynamic model;

        void before_each()
        {
            book = new Book();

            book.Title = "Title";

            book.Body = "Body";
        }

        void act_each()
        {
            book.IsValid();
        }

        void describe_first_error()
        {
            act = () => firstError = book.FirstError();

            context["both title and body are not specified in Book"] = () =>
            {
                before = () =>
                {
                    book.Title = string.Empty;
                    book.Body = string.Empty;
                };

                it["contains 2 errors"] = () => ((int)book.Errors().Count).should_be(2);

                it["first error should be Title is required"] = () => firstError.should_be("Title is required.");
            };

            context["title is specified but body is not"] = () =>
            {
                before = () =>
                {
                    book.Title = "Title";
                    book.Body = string.Empty;
                };

                it["contains 1 error"] = () => ((int)book.Errors().Count).should_be(1);

                it["first error should be Body is required"] = () => firstError.should_be("Body is required.");
            };
        }

        void case_insensitive_valdation()
        {
            act = () => isValid = book.IsValid();

            context["setting the title with a lower case T and body with lower case b"] = () =>
            {
                before = () => book = new Book(new { title = "Title", body = "Body" });

                it["is still valid"] = () => isValid.should_be_true();
            };
        }

        void describe_property_location()
        {
            context["initialization of entity with properties not defined in validation"] = () =>
            {
                before = () => book = new Book(new { id = 1 });

                it["it exists on prototype object as opposed to virtual"] = () => (book as DynamicModel).RespondsTo("id").should_be_true();
            };
        }

        void virtual_properties_for_validation()
        {
            context["when validation is mixed in to a dynamic model"] = () =>
            {
                before = () =>
                {
                    model = new DynamicModel();
                    model.Init();
                };

                it["the properties added for validation are virtual, so that they are not included in persistance by DynamicRepository"] = () =>
                {
                    var virtualProperties = (model as DynamicModel).UnTrackedProperties() as IDictionary<string, object>;

                    virtualProperties.Keys.should_contain("Errors");

                    virtualProperties.Keys.should_contain("IsValid");

                    virtualProperties.Keys.should_contain("IsPropertyValid");

                    virtualProperties.Keys.should_contain("FirstError");
                };
            };
        }
    }
}