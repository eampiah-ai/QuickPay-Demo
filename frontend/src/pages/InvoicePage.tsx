import { FormProvider, useForm } from "react-hook-form";
import type { Invoice } from "../components/InvoiceList";
import { Input } from "../components/ui/input";
import { Button } from "../components/ui/button";
import { Label } from "../components/ui/label";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useCallback, useEffect, useMemo } from "react";
import { RHFSelect, type SelectOption } from "../components/ui/RHFSelect";
import { Textarea } from "../components/ui/textarea";
import { useNavigate, useParams } from "react-router-dom";
import axios, { type AxiosResponse } from "axios";
import { format } from "date-fns";
import { IoArrowBackCircle } from "react-icons/io5";

const fetchCustomers = async () =>
  await axios
    .get<any, AxiosResponse<Customer[]>>("http://localhost:5250/api/customers")
    .then((response) => response.data);

const isNewId = (id?: string) => id === "new";

const fetchInvoice = async (id?: string) => {
  if (isNewId(id)) return {} as Invoice;

  return await axios
    .get<any, AxiosResponse<Invoice>>(
      `http://localhost:5250/api/invoices/${id}`
    )
    .then((response) => response.data);
};

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

  const { data: customers } = useQuery({
    queryKey: ["customers"],
    queryFn: fetchCustomers,
  });

  const customerSelect = useMemo(() => {
    if (!customers?.length) return;
    const customerOptions: SelectOption[] = customers.map(
      (customer: Customer) => {
        return { label: customer.name, value: customer.id };
      }
    );

    return (
      <div className="flex flex-col gap-2">
        <Label htmlFor="customerId">Customer</Label>
        <RHFSelect
          name="customerId"
          options={customerOptions}
          placeholder="Choose a customer"
          value={invoice?.customerId}
        />
      </div>
    );
  }, [customers, invoice]);

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
      <div className="flex justify-between items-center w-full mb-6">
        <IoArrowBackCircle
          className="cursor-pointer text-[2em]"
          onClick={() => navigate(-1)}
        />
        <div className="text-[3em] font-bold text-gray-800 flex-grow text-center">
          {invoice?.publicId}
        </div>
      </div>
      <FormProvider {...methods}>
        <form
          className="flex flex-col gap-7"
          onSubmit={methods.handleSubmit(submitInvoice)}
        >
          <div className="flex flex-col gap-2">
            <Label htmlFor="amount">Amount</Label>
            <Input
              type="number"
              id="amount"
              placeholder="Amount"
              {...methods.register("amountCents")}
            />
          </div>
          <div className="flex flex-col gap-2">
            <Label htmlFor="dueDate">Due Date</Label>
            <Input
              type="date"
              id="dueDate"
              placeholder="Due Date"
              {...methods.register("dueDate")}
            />
          </div>
          {customerSelect}
          <div className="flex flex-col gap-2">
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              placeholder="Description"
              {...methods.register("description")}
            />
          </div>
          <Button type="submit" className="cursor-pointer">
            Save
          </Button>
        </form>
      </FormProvider>
    </div>
  );
}
