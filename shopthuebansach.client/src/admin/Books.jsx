import React, { useEffect, useState } from "react";
import axios from "axios";

const apiBase = "/api/SaleBook";

export default function Books() {
    const [books, setBooks] = useState([]);
    const [editingBook, setEditingBook] = useState(null);
    const [form, setForm] = useState({
        SaleBookId: "",
        Title: "",
        AuthorId: "",
        CategoryId: "",
        Price: 0,
        DuocHienThi: true,
    });

    const fetchBooks = async () => {
        try {
            const res = await axios.get(apiBase);
            setBooks(res.data);
        } catch (error) {
            alert("Lỗi lấy sách bán");
        }
    };

    useEffect(() => {
        fetchBooks();
    }, []);

    const resetForm = () => {
        setForm({
            SaleBookId: "",
            Title: "",
            AuthorId: "",
            CategoryId: "",
            Price: 0,
            DuocHienThi: true,
        });
        setEditingBook(null);
    };

    const onChange = (e) => {
        const { name, value, type, checked } = e.target;
        setForm((f) => ({
            ...f,
            [name]: type === "checkbox" ? checked : value,
        }));
    };

    const onSubmit = async (e) => {
        e.preventDefault();
        try {
            if (editingBook) {
                // update
                await axios.put(`${apiBase}/${form.SaleBookId}`, form);
                alert("Cập nhật thành công");
            } else {
                // create
                await axios.post(apiBase, form);
                alert("Thêm sách thành công");
            }
            fetchBooks();
            resetForm();
        } catch (error) {
            alert("Lỗi khi lưu sách");
        }
    };

    const onEdit = (book) => {
        setForm(book);
        setEditingBook(book.SaleBookId);
    };

    const onDelete = async (id) => {
        if (!window.confirm("Bạn chắc chắn muốn xóa?")) return;
        try {
            await axios.delete(`${apiBase}/${id}`);
            alert("Xóa thành công");
            fetchBooks();
        } catch (error) {
            alert("Lỗi khi xóa sách");
        }
    };

    const onToggleVisibility = async (id, visible) => {
        try {
            await axios.put(`${apiBase}/hideorshow/${id}?visible=${visible}`);
            fetchBooks();
        } catch (error) {
            alert("Lỗi khi cập nhật trạng thái hiển thị");
        }
    };

    return (
        <div>
            <h2>Quản lý Sách Bán</h2>
            <form onSubmit={onSubmit}>
                <input
                    name="SaleBookId"
                    placeholder="ID sách"
                    value={form.SaleBookId}
                    onChange={onChange}
                    disabled={editingBook !== null}
                    required
                />
                <input
                    name="Title"
                    placeholder="Tên sách"
                    value={form.Title}
                    onChange={onChange}
                    required
                />
                <input
                    name="AuthorId"
                    placeholder="ID tác giả"
                    value={form.AuthorId}
                    onChange={onChange}
                    required
                />
                <input
                    name="CategoryId"
                    placeholder="ID thể loại"
                    value={form.CategoryId}
                    onChange={onChange}
                    required
                />
                <input
                    type="number"
                    name="Price"
                    placeholder="Giá bán"
                    value={form.Price}
                    onChange={onChange}
                    required
                />
                <label>
                    Hiển thị:
                    <input
                        type="checkbox"
                        name="DuocHienThi"
                        checked={form.DuocHienThi}
                        onChange={onChange}
                    />
                </label>
                <button type="submit">{editingBook ? "Cập nhật" : "Thêm"}</button>
                {editingBook && <button onClick={resetForm}>Hủy</button>}
            </form>

            <table border="1" cellPadding={5} style={{ marginTop: 20 }}>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Tên sách</th>
                        <th>Tác giả</th>
                        <th>Thể loại</th>
                        <th>Giá bán</th>
                        <th>Hiển thị</th>
                        <th>Hành động</th>
                    </tr>
                </thead>
                <tbody>
                    {books.map((b) => (
                        <tr key={b.SaleBookId}>
                            <td>{b.SaleBookId}</td>
                            <td>{b.Title}</td>
                            <td>{b.AuthorId}</td>
                            <td>{b.CategoryId}</td>
                            <td>{b.Price}</td>
                            <td>
                                <input
                                    type="checkbox"
                                    checked={b.DuocHienThi}
                                    onChange={() => onToggleVisibility(b.SaleBookId, !b.DuocHienThi)}
                                />
                            </td>
                            <td>
                                <button onClick={() => onEdit(b)}>Sửa</button>{" "}
                                <button onClick={() => onDelete(b.SaleBookId)}>Xóa</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
