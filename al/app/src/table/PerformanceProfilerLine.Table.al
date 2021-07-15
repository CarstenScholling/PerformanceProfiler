table 98991 "Performance Profiler Line"
{
    Caption = 'Performance Profiler Line';
    DataClassification = CustomerContent;

    fields
    {
        field(10; "Performance Profiler Code"; Code[20])
        {
            Caption = 'Performance Profiler Code';
            DataClassification = CustomerContent;
        }

        field(20; "Entry No."; Integer)
        {
            Caption = 'Entry No.';
            DataClassification = SystemMetadata;
        }

        field(100; "Session Id"; Integer)
        {
            Caption = 'Session Id';
            DataClassification = SystemMetadata;
        }

        field(110; "User Name"; Text[50])
        {
            Caption = 'User Name';
            DataClassification = EndUserIdentifiableInformation;
        }

        field(120; Tenant; Text[128])
        {
            Caption = 'Tenant';
            DataClassification = OrganizationIdentifiableInformation;
        }

        field(130; "App Id"; Guid)
        {
            Caption = 'App Id';
            DataClassification = SystemMetadata;
        }

        field(140; "App Name"; Text[150])
        {
            Caption = 'App Name';
            Editable = false;
        }

        field(150; Identation; Integer)
        {
            Caption = 'Identation';
            DataClassification = SystemMetadata;
        }

        field(160; "Object Type"; Enum "ETW Object Type")
        {
            Caption = 'Object Type';
            DataClassification = SystemMetadata;
        }

        field(165; "Object Type Name"; Text[50])
        {
            Caption = 'Object Type Name';
            DataClassification = SystemMetadata;
        }

        field(170; "Object Id"; Integer)
        {
            Caption = 'Object Id';
            BlankZero = true;
            DataClassification = SystemMetadata;
        }

        field(180; "Object Name"; Text[50])
        {
            Caption = 'Object Name';
            Editable = false;
            FieldClass = FlowField;
            CalcFormula = lookup(AllObj."Object Name" where("Object Type" = field("Object Type"), "Object ID" = field("Object Id")));
        }

        field(190; "Statement Line No."; Integer)
        {
            Caption = 'Line No.';
            BlankZero = true;
            DataClassification = SystemMetadata;
        }

        field(200; Statement; Text[2048])
        {
            Caption = 'Statement';
            DataClassification = CustomerContent;
        }

        field(210; "Full Statement"; Blob)
        {
            Caption = 'Full Statement';
            DataClassification = CustomerContent;
        }

        field(220; "Duration (ms)"; Integer)
        {
            Caption = 'Duration (ms)';
            BlankZero = true;
            DataClassification = SystemMetadata;
        }

        field(230; "Min. Duration"; BigInteger)
        {
            Caption = 'Min. Duration';
            BlankZero = true;
            DataClassification = SystemMetadata;
        }

        field(240; "Max. Duration"; BigInteger)
        {
            Caption = 'Max. Duration';
            BlankZero = true;
            DataClassification = SystemMetadata;
        }

        field(250; "Last Active (ms)"; BigInteger)
        {
            Caption = 'Last Active (ms)';
            BlankZero = true;
            DataClassification = SystemMetadata;
        }

        field(260; "Hit Count"; Integer)
        {
            Caption = 'Hit Count';
            BlankZero = true;
            DataClassification = SystemMetadata;
        }
    }

    keys
    {
        key(PK; "Performance Profiler Code", "Entry No.")
        {
            Clustered = true;
        }
    }

    procedure SetStatement(newStatement: Text)
    var
        toStream: OutStream;
    begin
        Statement := CopyStr(newStatement, 1, MaxStrLen(Statement));

        "Full Statement".CreateOutStream(toStream);
        toStream.WriteText(newStatement);
    end;

    procedure GetStatement(): Text
    var
        fromStream: InStream;
        returnStatement: Text;
    begin
        "Full Statement".CreateInStream(fromStream);
        fromStream.ReadText(returnStatement);

        exit(returnStatement);
    end;
}
