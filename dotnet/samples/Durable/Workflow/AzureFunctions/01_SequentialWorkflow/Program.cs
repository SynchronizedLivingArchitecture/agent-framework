// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Agents.AI.Workflows;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Agents.AI.Hosting.AzureFunctions;
using Microsoft.Extensions.Hosting;
using SequentialWorkflow;

OrderLookup orderLookup = new();
OrderCancel orderCancel = new();
SendEmail sendEmail = new();

// Build the CancelOrder workflow: OrderLookup -> OrderCancel -> SendEmail
Workflow cancelOrder = new WorkflowBuilder(orderLookup)
    .WithName("CancelOrder")
    .WithDescription("Cancel an order and notify the customer")
    .AddEdge(orderLookup, orderCancel)
    .AddEdge(orderCancel, sendEmail)
    .Build();

// Configure the function app to host the AI agent.
// This will automatically generate HTTP API endpoints for the agent.
using IHost app = FunctionsApplication
    .CreateBuilder(args)
    .ConfigureFunctionsWebApplication()
    .ConfigureDurableWorkflows(options => options.AddWorkflow(cancelOrder))
    //  .ConfigureDurableAgents(options => options.AddAIAgent(agent, timeToLive: TimeSpan.FromHours(1)))
    .Build();
app.Run();
