﻿var template = "";

$(function () {
  $("#saveAll").click(SaveAll);
  $("#new").click(NewPerson);
  $(".close").click(hideSaveSuccessful);
  template = $("#peepTemplate").html();
  hideSaveSuccessful();
  GetPeople();
});

function hideSaveSuccessful() { $("#saveSuccessful").hide(); }

function showSaveSuccessful() { $("#saveSuccessful").show(); }

function NewPerson() {
  var control = $(template);
  bindControl(control, { name: "" });
  control.find("input.personname").keyup(function () {
    control.find(".preview").html($(this).val());
  });
}

function bindControl(control, data) {
  control.find("input.personname").val(data.name);
  control.find("input.save").click(Save);
  control.find(".preview").html(data.name);
  $("#peeps").append(control);
}

function Save() {
  var element = this;
  var person = PersonFor($(this).parent());
  $.post("/home/update", person, function (data) {
    $(element).parent().attr("data-id", data.id)
    showSaveSuccessful();
  });
}

function GetPeople() {
  $("#peeps").html('');
  $.getJSON('/home/list', function (data) {
    for (var i = 0; i < data.length; i++) {
      var control = $(template);
      control.attr("data-id", data[i].id);
      bindControl(control, data[i]);
      control.find("input.personname").keyup(function () {
        $(this).parent().find(".preview").html($(this).val());
      });
    }
  });
}

function PersonFor(element) {
  var name = $(element).find("input.personname").val();
  var id = $(element).attr("data-id");
  var person = { name: name };
  if (id) person.id = id;
  return person;
}

function SaveAll() {
  var people = [];

  $("#peeps .person").each(function () {
    people.push(PersonFor(this));
  });

  $.ajax({
    type: 'POST',
    url: "/home/updateall",
    data: JSON.stringify(people),
    success: function () {
      showSaveSuccessful();
      GetPeople(); 
    },
    contentType: 'application/json'
  });
}
