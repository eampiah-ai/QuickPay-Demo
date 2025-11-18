import {
  flexRender,
  getCoreRowModel,
  getPaginationRowModel,
  useReactTable,
  type ColumnDef,
} from "@tanstack/react-table";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "./ui/table";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { Button } from "./ui/button";
import { useNavigate, useSearchParams } from "react-router-dom";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "./ui/dropdown-menu";
import { MoreHorizontal } from "lucide-react";
import { useEffect, useRef } from "react";
import { toast } from "sonner";
import { format } from "date-fns";
import { MdAddCircle } from "react-icons/md";
import axios, { type AxiosResponse } from "axios";

export interface Invoice {
  id: string;
  publicId: string;
  customerId: string;
  amountCents: number;
  description?: string;
  status: string;
  dueDate: string;
  createdAt?: string;
}

export const decimalFormatter = new Intl.NumberFormat("en-us", {
  style: "decimal",
  minimumFractionDigits: 2,
  maximumFractionDigits: 2,
  useGrouping: true,
});

const fetchInvoices = async () =>
  await axios
    .get<any, AxiosResponse<Invoice[]>>("http://localhost:5250/api/invoices")
    .then((response) => response.data);

export default function InvoiceList() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const [searchParams, setSearchParams] = useSearchParams();
  const toastType = searchParams.get("toast");
  const message = searchParams.get("message");
  const didShowToast = useRef(false);

  useEffect(() => {
    if (didShowToast.current) return;
    if (toastType && message) {
      didShowToast.current = true;
      switch (toastType) {
        case "success":
          toast.success(message);
          break;
        case "error":
          toast.error(message);
          break;
        default:
          toast(message);
          break;
      }
      queryClient.invalidateQueries({ queryKey: ["invoices"] });
      const newSearchParams = new URLSearchParams(searchParams);
      newSearchParams.delete("toast");
      newSearchParams.delete("message");

      setSearchParams(newSearchParams, { replace: true });
    }
  }, [toastType, message]);

  const { data, isPending, isError, refetch } = useQuery({
    queryKey: ["invoices"],
    queryFn: fetchInvoices,
    initialData: [],
    staleTime: 0,
  });

  const deleteInvoice = (invoiceId: string) => {
    const request = new Request(
      `http://localhost:5250/api/invoices/${invoiceId}`,
      {
        method: "DELETE",
      }
    );

    fetch(request).then((response) => {
      if (!response.ok) {
        console.log("Error deleting invoice");
        return;
      }

      refetch();
    });
  };

  const makePayment = async (invoiceId: string) => {
    const url = await axios
      .get<any, AxiosResponse<string>>(
        `http://localhost:5250/api/payments/${invoiceId}/create-checkout-session`
      )
      .then((response) => response.data);
    window.location.href = url;
  };

  const columns: ColumnDef<Invoice>[] = [
    { accessorKey: "publicId", header: "ID" },
    {
      accessorKey: "amountCents",
      header: "Amount",
      cell: ({ row }) => {
        return (
          <div>${decimalFormatter.format(row.original.amountCents / 100)}</div>
        );
      },
    },
    {
      accessorKey: "dueDate",
      header: "Due Date",
      cell: ({ row }) => {
        const dateValue: string = row.getValue("dueDate");
        if (!dateValue) {
          return "N/A";
        }

        return (
          <div className="font-medium">{format(dateValue, "MMM dd, yyyy")}</div>
        );
      },
    },
    { accessorKey: "status", header: "Status" },
    {
      id: "actions",
      cell: ({ row }) => {
        const invoice = row.original;

        return (
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="ghost" className="h-8 w-8 p-0">
                <span className="sr-only">Open menu</span>
                <MoreHorizontal className="h-4 w-4" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuLabel>Actions</DropdownMenuLabel>
              <DropdownMenuItem
                onClick={() => navigate(`/invoices/${invoice.id}`)}
              >
                View invoice details
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => deleteInvoice(invoice.id)}>
                Delete
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={() => makePayment(invoice.publicId)}>
                Make payment
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        );
      },
    },
  ];

  const table = useReactTable<Invoice>({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
  });

  if (isPending) return <div>Loading...</div>;
  if (isError) return <div>An error occurred.</div>;

  return (
    <div className="p-5">
      <div className="flex flex-row items-center gap-5">
        <h1 className="text-[3em]">Invoices</h1>
        <Button
          className="cursor-pointer"
          onClick={() => navigate("/invoices/new")}
        >
          <MdAddCircle />
          New
        </Button>
      </div>
      <Table>
        <TableHeader>
          {table.getHeaderGroups().map((headerGroup) => (
            <TableRow key={headerGroup.id}>
              {headerGroup.headers.map((header) => {
                return (
                  <TableHead key={header.id}>
                    {header.isPlaceholder
                      ? null
                      : flexRender(
                          header.column.columnDef.header,
                          header.getContext()
                        )}
                  </TableHead>
                );
              })}
            </TableRow>
          ))}
        </TableHeader>
        <TableBody>
          {table.getRowModel().rows?.length ? (
            table.getRowModel().rows.map((row) => (
              <TableRow
                key={row.id}
                data-state={row.getIsSelected() && "selected"}
              >
                {row.getVisibleCells().map((cell) => (
                  <TableCell key={cell.id}>
                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                  </TableCell>
                ))}
              </TableRow>
            ))
          ) : (
            <TableRow>
              <TableCell colSpan={columns.length} className="h-24 text-center">
                No results.
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </div>
  );
}
