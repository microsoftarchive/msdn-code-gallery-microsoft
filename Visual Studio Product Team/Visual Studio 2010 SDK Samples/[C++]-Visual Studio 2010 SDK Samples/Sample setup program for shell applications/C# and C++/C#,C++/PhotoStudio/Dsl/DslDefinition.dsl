<?xml version="1.0" encoding="UTF-8"?>
<!--                                                                  -->
<!-- Copyright (c) Microsoft Corporation. All rights reserved.        -->
<!-- This code is licensed under the Visual Studio SDK license terms. -->
<!-- THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF                -->
<!-- ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY               -->
<!-- IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR                   -->
<!-- PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.                   -->
<!--                                                                  -->
<Dsl xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="d33c6887-9f79-4fe0-a6a8-c3b7203e756d" Description="Description for Company.SlideShowDesigner.SlideShowDesigner" Name="SlideShowDesigner" DisplayName="SlideShowDesigner" Namespace="Company.SlideShowDesigner" ProductName="SlideShowDesigner" CompanyName="Company" PackageGuid="6d5da338-d523-4ecd-990d-1e397748d2ba" PackageNamespace="Company.SlideShowDesigner" xmlns="http://schemas.microsoft.com/VisualStudio/2005/DslTools/DslDefinitionModel">
  <Classes>
    <DomainClass Id="cfa18b1b-fcd6-451a-ab70-1fb3c8381b7a" Description="The root in which all other elements are embedded. Appears as a diagram." Name="ExampleModel" DisplayName="Example Model" Namespace="Company.SlideShowDesigner">
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Notes>Creates an embedding link when an element is dropped onto a model. </Notes>
          <Index>
            <DomainClassMoniker Name="Photo" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ExampleModelHasElements.Elements</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="983dcf38-57b4-41f4-8cc5-18683f5fc217" Description="Elements embedded in the model. Appear as boxes on the diagram." Name="Photo" DisplayName="Photo" Namespace="Company.SlideShowDesigner">
      <Properties>
        <DomainProperty Id="b56dd0e1-011c-48e0-ac95-1b4dbaf563bd" Description="Description for Company.SlideShowDesigner.Photo.Name" Name="Name" DisplayName="Name" DefaultValue="" IsElementName="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
  </Classes>
  <Relationships>
    <DomainRelationship Id="5e1b2d04-d152-4e0a-95b2-2579ee0967f9" Description="Embedding relationship between the Model and Elements" Name="ExampleModelHasElements" DisplayName="Example Model Has Elements" Namespace="Company.SlideShowDesigner" IsEmbedding="true">
      <Source>
        <DomainRole Id="24958c4e-328a-4255-99e0-ba9b69d6d1ef" Description="" Name="ExampleModel" DisplayName="Example Model" PropertyName="Elements" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Elements">
          <RolePlayer>
            <DomainClassMoniker Name="ExampleModel" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="44ef65b7-fc9a-4203-9457-167e4d478f9b" Description="" Name="Element" DisplayName="Element" PropertyName="ExampleModel" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Example Model">
          <RolePlayer>
            <DomainClassMoniker Name="Photo" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="bc01f84f-c65c-4bfa-b5fb-1f71459e1532" Description="Reference relationship between Elements." Name="PhotoReferencesTargets" DisplayName="Photo References Targets" Namespace="Company.SlideShowDesigner">
      <Source>
        <DomainRole Id="1880609d-9384-42b9-8020-89cfd97a5bc2" Description="Description for Company.SlideShowDesigner.ExampleRelationship.Target" Name="Source" DisplayName="Source" PropertyName="Targets" PropertyDisplayName="Targets">
          <RolePlayer>
            <DomainClassMoniker Name="Photo" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="1fc535ee-738c-4915-8035-04ffa774da00" Description="Description for Company.SlideShowDesigner.ExampleRelationship.Source" Name="Target" DisplayName="Target" PropertyName="Sources" PropertyDisplayName="Sources">
          <RolePlayer>
            <DomainClassMoniker Name="Photo" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
  </Relationships>
  <Types>
    <ExternalType Name="DateTime" Namespace="System" />
    <ExternalType Name="String" Namespace="System" />
    <ExternalType Name="Int16" Namespace="System" />
    <ExternalType Name="Int32" Namespace="System" />
    <ExternalType Name="Int64" Namespace="System" />
    <ExternalType Name="UInt16" Namespace="System" />
    <ExternalType Name="UInt32" Namespace="System" />
    <ExternalType Name="UInt64" Namespace="System" />
    <ExternalType Name="SByte" Namespace="System" />
    <ExternalType Name="Byte" Namespace="System" />
    <ExternalType Name="Double" Namespace="System" />
    <ExternalType Name="Single" Namespace="System" />
    <ExternalType Name="Guid" Namespace="System" />
    <ExternalType Name="Boolean" Namespace="System" />
    <ExternalType Name="Char" Namespace="System" />
  </Types>
  <Shapes>
    <ImageShape Id="31dc9b1d-5cd4-4b31-a156-492967cd9c33" Description="Description for Company.SlideShowDesigner.PhotoShape" Name="PhotoShape" DisplayName="Photo Shape" Namespace="Company.SlideShowDesigner" GeneratesDoubleDerived="true" FixedTooltipText="Photo Shape" InitialHeight="1" Image="..\..\SamplePictures\Penguins.jpg">
      <Properties>
        <DomainProperty Id="113f948a-15d8-478c-9e73-b32b5b00199d" Description="Description for Company.SlideShowDesigner.PhotoShape.Image Path" Name="ImagePath" DisplayName="Image Path">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <ShapeHasDecorators Position="OuterBottomCenter" HorizontalOffset="0" VerticalOffset="0">
        <TextDecorator Name="NameDecorator" DisplayName="Name Decorator" DefaultText="NameDecorator" />
      </ShapeHasDecorators>
    </ImageShape>
  </Shapes>
  <Connectors>
    <Connector Id="01de9838-04c9-465c-9695-cc72c36dab42" Description="Connector between the ExampleShapes. Represents ExampleRelationships on the Diagram." Name="Transition" DisplayName="Transition" Namespace="Company.SlideShowDesigner" FixedTooltipText="Transition" Color="113, 111, 110" TargetEndStyle="EmptyArrow" Thickness="0.01" />
  </Connectors>
  <XmlSerializationBehavior Name="SlideShowDesignerSerializationBehavior" Namespace="Company.SlideShowDesigner">
    <ClassData>
      <XmlClassData TypeName="ExampleModel" MonikerAttributeName="" SerializeId="true" MonikerElementName="exampleModelMoniker" ElementName="exampleModel" MonikerTypeName="ExampleModelMoniker">
        <DomainClassMoniker Name="ExampleModel" />
        <ElementData>
          <XmlRelationshipData RoleElementName="elements">
            <DomainRelationshipMoniker Name="ExampleModelHasElements" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="Photo" MonikerAttributeName="name" MonikerElementName="photoMoniker" ElementName="photo" MonikerTypeName="PhotoMoniker">
        <DomainClassMoniker Name="Photo" />
        <ElementData>
          <XmlPropertyData XmlName="name" IsMonikerKey="true">
            <DomainPropertyMoniker Name="Photo/Name" />
          </XmlPropertyData>
          <XmlRelationshipData RoleElementName="targets">
            <DomainRelationshipMoniker Name="PhotoReferencesTargets" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ExampleModelHasElements" MonikerAttributeName="" MonikerElementName="exampleModelHasElementsMoniker" ElementName="exampleModelHasElements" MonikerTypeName="ExampleModelHasElementsMoniker">
        <DomainRelationshipMoniker Name="ExampleModelHasElements" />
      </XmlClassData>
      <XmlClassData TypeName="PhotoReferencesTargets" MonikerAttributeName="" MonikerElementName="photoReferencesTargetsMoniker" ElementName="photoReferencesTargets" MonikerTypeName="PhotoReferencesTargetsMoniker">
        <DomainRelationshipMoniker Name="PhotoReferencesTargets" />
      </XmlClassData>
      <XmlClassData TypeName="Transition" MonikerAttributeName="" MonikerElementName="transitionMoniker" ElementName="transition" MonikerTypeName="TransitionMoniker">
        <ConnectorMoniker Name="Transition" />
      </XmlClassData>
      <XmlClassData TypeName="SlideShowDesignerDiagram" MonikerAttributeName="" MonikerElementName="minimalLanguageDiagramMoniker" ElementName="minimalLanguageDiagram" MonikerTypeName="SlideShowDesignerDiagramMoniker">
        <DiagramMoniker Name="SlideShowDesignerDiagram" />
      </XmlClassData>
      <XmlClassData TypeName="PhotoShape" MonikerAttributeName="" MonikerElementName="photoShapeMoniker" ElementName="photoShape" MonikerTypeName="PhotoShapeMoniker">
        <ImageShapeMoniker Name="PhotoShape" />
        <ElementData>
          <XmlPropertyData XmlName="imagePath">
            <DomainPropertyMoniker Name="PhotoShape/ImagePath" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
    </ClassData>
  </XmlSerializationBehavior>
  <ExplorerBehavior Name="SlideShowDesignerExplorer" />
  <ConnectionBuilders>
    <ConnectionBuilder Name="PhotoReferencesTargetsBuilder">
      <Notes>Provides for the creation of an ExampleRelationship by pointing at two ExampleElements.</Notes>
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="PhotoReferencesTargets" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="Photo" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="Photo" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
  </ConnectionBuilders>
  <Diagram Id="690c3f6e-89d1-44a2-a025-32f966967230" Description="Description for Company.SlideShowDesigner.SlideShowDesignerDiagram" Name="SlideShowDesignerDiagram" DisplayName="Minimal Language Diagram" Namespace="Company.SlideShowDesigner">
    <Class>
      <DomainClassMoniker Name="ExampleModel" />
    </Class>
    <ShapeMaps>
      <ShapeMap>
        <DomainClassMoniker Name="Photo" />
        <ParentElementPath>
          <DomainPath>ExampleModelHasElements.ExampleModel/!ExampleModel</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="PhotoShape/NameDecorator" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Photo/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <ImageShapeMoniker Name="PhotoShape" />
      </ShapeMap>
    </ShapeMaps>
    <ConnectorMaps>
      <ConnectorMap>
        <ConnectorMoniker Name="Transition" />
        <DomainRelationshipMoniker Name="PhotoReferencesTargets" />
      </ConnectorMap>
    </ConnectorMaps>
  </Diagram>
  <Designer CopyPasteGeneration="CopyPasteOnly" FileExtension="ssd" EditorGuid="ce7aa2b5-c9ee-494b-91ec-a26c6ddfcbea">
    <RootClass>
      <DomainClassMoniker Name="ExampleModel" />
    </RootClass>
    <XmlSerializationDefinition CustomPostLoad="false">
      <XmlSerializationBehaviorMoniker Name="SlideShowDesignerSerializationBehavior" />
    </XmlSerializationDefinition>
    <ToolboxTab TabText="SlideShowDesigner">
      <ElementTool Name="Photo" ToolboxIcon="resources\exampleshapetoolbitmap.bmp" Caption="Photo" Tooltip="Create an ExampleElement" HelpKeyword="CreateExampleClassF1Keyword">
        <DomainClassMoniker Name="Photo" />
      </ElementTool>
      <ConnectionTool Name="Transition" ToolboxIcon="resources\exampleconnectortoolbitmap.bmp" Caption="Transition" Tooltip="Drag between ExampleElements to create an ExampleRelationship" HelpKeyword="ConnectExampleRelationF1Keyword">
        <ConnectionBuilderMoniker Name="SlideShowDesigner/PhotoReferencesTargetsBuilder" />
      </ConnectionTool>
    </ToolboxTab>
    <Validation UsesMenu="false" UsesOpen="false" UsesSave="false" UsesLoad="false" />
    <DiagramMoniker Name="SlideShowDesignerDiagram" />
  </Designer>
  <Explorer ExplorerGuid="d129a62a-a4d7-4116-a269-9a04a0e5063e" Title="SlideShowDesigner Explorer">
    <ExplorerBehaviorMoniker Name="SlideShowDesigner/SlideShowDesignerExplorer" />
  </Explorer>
</Dsl>