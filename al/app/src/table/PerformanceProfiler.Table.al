table 98990 "Performance Profiler"
{
    Caption = 'Performance Profiler';
    DataClassification = ToBeClassified;
    LookupPageId = "Performance Profiler List";
    DrillDownPageId = "Performance Profiler List";

    fields
    {
        field(10; Code; Code[20])
        {
            Caption = 'Code';
            DataClassification = CustomerContent;
        }

        field(100; Description; Text[100])
        {
            Caption = 'Description';
            DataClassification = CustomerContent;
        }

        field(110; "No. of Lines"; Integer)
        {
            Caption = 'No. of Lines';
            BlankZero = true;
            Editable = false;
            FieldClass = FlowField;
            CalcFormula = count("Performance Profiler Line" where("Performance Profiler Code" = field(Code)));
        }

        field(200; "Session Id"; Integer)
        {
            Caption = 'Session ID';
            BlankZero = true;
            MinValue = -1;
            TableRelation = "Active Session"."Session ID";
            ValidateTableRelation = false;

            trigger OnLookup()
            var
                activeSession: Record "Active Session";
            begin
                if Page.RunModal(Page::"Concurrent Session List", activeSession) = Action::LookupOK then begin
                    "Session Id" := activeSession."Session ID";
                end;
            end;
        }

        field(210; Threshold; Integer)
        {
            Caption = 'Threshold';
            BlankZero = true;
            MinValue = 0;
        }

        field(220; "App Id"; Guid)
        {
            Caption = 'App ID';
            TableRelation = "Published Application".ID;
            ValidateTableRelation = false;

            trigger OnValidate()
            begin
                CalcFields("App Name");
            end;
        }

        field(230; "App Name"; Text[150])
        {
            Caption = 'App Name';
            Editable = false;
            FieldClass = FlowField;
            CalcFormula = lookup("Published Application".Name where(ID = field("App Id")));
        }

    }

    keys
    {
        key(PK; Code)
        {
            Clustered = true;
        }
    }

    fieldgroups
    {
        fieldgroup(DropDown; Code, Description)
        {
        }
    }
}
