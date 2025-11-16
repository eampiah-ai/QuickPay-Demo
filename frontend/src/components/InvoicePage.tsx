import { FormProvider, useForm } from "react-hook-form";
import type { Invoice } from "./InvoiceList";
import { Input } from "./ui/input";
import { Button } from "./ui/button";
import { Label } from "./ui/label";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useCallback, useEffect, useMemo, useState } from "react";
import { RHFSelect } from "./ui/RHFSelect";
import { Textarea } from "./ui/textarea";
import { useNavigate, useParams } from "react-router-dom";
import axios from "axios";
import { toast } from "sonner";
import { format } from "date-fns";

function fetchCustomers() {
  return fetch("http://localhost:5250/api/customers").then((res) => res.json());
}

const isNewId = (id?: string) => id === "new";

async function fetchInvoice(id?: string) {
  if (isNewId(id)) return {} as Invoice;

  const request = new Request(`http://localhost:5250/api/invoices/${id}`);
  return await fetch(request).then(async (response) => {
    if (!response.ok) {
      console.log("Error fetching invoice");
      return;
    }

    return await response.json();
  });
}

const createInvoice = async (invoice: Invoice) =>
  await axios.post("http://localhost:5250/api/invoices", invoice);

const updateInvoice = async (invoice: Invoice) =>
  await axios.put(`http://localhost:5250/api/invoices/${invoice.id}`, invoice);

interface Customer {
  id: string;
  name: string;
  email: string;
  createdAt: Date;
}

export default function InvoicePage() {
  const methods = useForm<Invoice>();
  const navigate = useNavigate();
  const { id } = useParams();

  const queryClient = useQueryClient();
  const {
    data: invoice,
    isPending: isInvoiceQueryPending,
    isError: isInvoiceQueryError,
  } = useQuery({ queryKey: ["invoice", id], queryFn: () => fetchInvoice(id) });

  const mutationCallback = useCallback((url: string) => {
    queryClient.invalidateQueries({ queryKey: ["invoice", id] });
    navigate(url);
  }, []);

  const createMutation = useMutation({
    mutationFn: (invoice: Invoice) => createInvoice(invoice),
    onSuccess: () =>
      mutationCallback(
        "/dashboard?toast=success&message=Successfully+created+invoice"
      ),
  });

  const updateMutation = useMutation({
    mutationFn: (invoice: Invoice) => updateInvoice(invoice),
    onSuccess: () =>
      mutationCallback(
        "/dashboard?toast=success&message=Successfully+updated+invoice"
      ),
  });

  const { data } = useQuery({
    queryKey: ["customers"],
    queryFn: fetchCustomers,
  });

  const customerSelect = useMemo(() => {
    if (!data?.length) return;
    const customerOptions = data.map((customer: Customer) => {
      return { label: customer.name, value: customer.id };
    });
    return (
      <RHFSelect
        name="customerId"
        options={customerOptions}
        label="Select Customer"
        placeholder="Choose a customer"
        value={invoice?.customerId}
      />
    );
  }, [data]);

  // set default values on fetch
  useEffect(() => {
    if (!invoice?.id) return;
    methods.reset({
      id: invoice.id,
      amountCents: invoice.amountCents,
      description: invoice.description,
      customerId: invoice.customerId,
      dueDate: format(invoice.dueDate, "yyyy-MM-dd"),
      publicId: invoice.publicId,
    });
  }, [invoice]);

  if (!isNewId(id) && isInvoiceQueryPending) return <div>Loading...</div>;
  if (!isNewId(id) && isInvoiceQueryError) return <div>An error occurred.</div>;

  const submitInvoice = (data: Invoice) => {
    if (isNewId(id)) {
      createMutation.mutate(data);
      return;
    } else if (id) {
      updateMutation.mutate(data);
    }
  };

  return (
    <div className="flex flex-col gap-5 p-5">
      <h1 className="text-xl text-center">Invoice</h1>
      <FormProvider {...methods}>
        <form
          className="flex flex-col gap-5"
          onSubmit={methods.handleSubmit(submitInvoice)}
        >
          <Label htmlFor="amount">Amount</Label>
          <Input
            type="number"
            id="amount"
            placeholder="Amount"
            {...methods.register("amountCents")}
          />
          <Label htmlFor="dueDate">Due Date</Label>
          <Input
            type="date"
            id="dueDate"
            placeholder="Due Date"
            {...methods.register("dueDate")}
          />
          {customerSelect}
          <Textarea
            placeholder="Description"
            {...methods.register("description")}
          />
          <Button type="submit" className="cursor-pointer">
            Save
          </Button>
        </form>
      </FormProvider>
    </div>
  );
}
